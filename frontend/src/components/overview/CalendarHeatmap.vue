<template>
  <div class="panel rounded-2xl p-6 bg-[#1D1F22]/80 border border-[#35383E] shadow-lg">
    <h2 class="text-lg font-semibold text-white mb-4">
      📅 Spending Calendar Heatmap
    </h2>

    <p class="text-gray-400 text-sm mb-4">
      Showing spending for the selected date range.
    </p>

    <!-- Empty state -->
    <div v-if="weeks.length === 0" class="text-gray-500 text-center py-6">
      No spending data for this range.
    </div>

    <!-- Heatmap -->
    <div v-else class="w-full overflow-x-auto pb-3">
      <div class="inline-block">
        <!-- Day-of-week header -->
        <div class="flex items-center gap-2 mb-2">
          <span class="w-12"></span>
          <div class="flex gap-1">
            <span
              v-for="d in dayLabels"
              :key="d"
              class="w-6 text-[10px] text-gray-500 text-center"
            >
              {{ d }}
            </span>
          </div>
        </div>

        <!-- Week rows -->
        <div
          v-for="(week, wi) in weeks"
          :key="wi"
          class="flex items-center gap-2 mb-1"
        >
          <!-- Week label (start date of week) -->
          <span class="w-12 text-[10px] text-gray-500">
            {{ formatWeekLabel(week) }}
          </span>

          <!-- 7 days -->
          <div class="flex gap-1">
            <div
              v-for="(day, di) in week.days"
              :key="di"
              class="w-6 h-6 rounded-[4px] transition"
              :style="{ backgroundColor: colorScale(day.amount) }"
              @mouseenter="showTooltip(day, $event)"
              @mouseleave="hideTooltip"
            ></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tooltip -->
    <div
      v-if="tooltip.visible"
      class="tooltip-box"
      :style="{ top: tooltip.y + 'px', left: tooltip.x + 'px' }"
    >
      <p class="font-semibold text-white text-sm">
        {{ tooltip.date }}
      </p>
      <p class="text-indigo-400 text-xs">
        ${{ fmtAmt(tooltip.amount) }} spent
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  // Already filtered by the date range in DashboardView
  expenses: {
    type: Array,
    default: () => [],
  },
});

// Sun → Sat labels (horizontal)
const dayLabels = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

const weeks = ref([]); // [{ weekStart: Date, days: [{ date, amount }] }]

const DAY_MS = 24 * 60 * 60 * 1000;

function startOfDay(d) {
  const nd = new Date(d);
  nd.setHours(0, 0, 0, 0);
  return nd;
}

/**
 * Build vertical weekly heatmap from filtered expenses.
 * Each row = one week, each row has 7 day cells (Sun–Sat).
 */
function buildHeatmap() {
  const exp = Array.isArray(props.expenses) ? props.expenses : [];

  if (!exp.length) {
    weeks.value = [];
    return;
  }

  // dateStr -> total amount
  const map = {};
  for (const e of exp) {
    if (!e || !e.date) continue;
    const d = startOfDay(new Date(e.date));
    const key = d.toISOString().slice(0, 10);
    map[key] = (map[key] || 0) + Number(e.amount || 0);
  }

  // Get min/max dates in filtered range
  const sortedDates = exp
    .filter((e) => e && e.date)
    .map((e) => startOfDay(new Date(e.date)))
    .sort((a, b) => a - b);

  if (!sortedDates.length) {
    weeks.value = [];
    return;
  }

  const minDate = sortedDates[0];
  const maxDate = sortedDates[sortedDates.length - 1];

  // Align to whole weeks: start at previous Sunday, end at next Saturday
  const weekStart = new Date(minDate);
  weekStart.setDate(weekStart.getDate() - weekStart.getDay()); // 0 = Sunday

  const weekEnd = new Date(maxDate);
  weekEnd.setDate(weekEnd.getDate() + (6 - weekEnd.getDay())); // extend to Saturday

  const result = [];

  // Iterate from weekStart → weekEnd one day at a time
  for (
    let cursor = new Date(weekStart);
    cursor <= weekEnd;
    cursor = new Date(cursor.getTime() + DAY_MS)
  ) {
    const dayIdx = cursor.getDay(); // 0–6
    const diffDays = Math.round((cursor - weekStart) / DAY_MS);
    const weekIdx = Math.floor(diffDays / 7);

    if (!result[weekIdx]) {
      result[weekIdx] = {
        weekStart: new Date(
          weekStart.getTime() + weekIdx * 7 * DAY_MS
        ),
        days: new Array(7).fill(null),
      };
    }

    const key = startOfDay(cursor).toISOString().slice(0, 10);
    result[weekIdx].days[dayIdx] = {
      date: key,
      amount: map[key] || 0,
    };
  }

  // Fill any null day slots with 0 amount (so the grid stays rectangular)
  for (const w of result) {
    w.days = w.days.map((d, idx) => {
      if (d) return d;
      const placeholderDate = new Date(w.weekStart);
      placeholderDate.setDate(placeholderDate.getDate() + idx);
      return {
        date: placeholderDate.toISOString().slice(0, 10),
        amount: 0,
      };
    });
  }

  weeks.value = result;
}

// Rebuild whenever the filtered expenses change
watch(
  () => props.expenses,
  () => {
    buildHeatmap();
  },
  { deep: true, immediate: true }
);

/* ---------- Color scale ---------- */
function colorScale(value) {
  if (value <= 0) return "rgba(255,255,255,0.04)";
  if (value < 5) return "rgba(129,140,248,0.25)";
  if (value < 20) return "rgba(129,140,248,0.40)";
  if (value < 50) return "rgba(129,140,248,0.65)";
  return "rgba(129,140,248,0.9)";
}

/* ---------- Tooltip ---------- */
const tooltip = ref({
  visible: false,
  date: "",
  amount: 0,
  x: 0,
  y: 0,
});

function showTooltip(day, evt) {
  tooltip.value = {
    visible: true,
    date: new Date(day.date).toLocaleDateString(),
    amount: day.amount,
    x: evt.clientX + 10,
    y: evt.clientY + 10,
  };
}

function hideTooltip() {
  tooltip.value.visible = false;
}

const fmtAmt = (n) => Number(n || 0).toFixed(2);

/* ---------- Week label ---------- */
function formatWeekLabel(week) {
  if (!week || !week.weekStart) return "";
  const d = week.weekStart;
  return d.toLocaleDateString(undefined, {
    month: "short",
    day: "numeric",
  });
}
</script>

<style scoped>
.month-label {
  @apply text-gray-400 text-xs mb-1;
}
.tooltip-box {
  position: fixed;
  z-index: 50;
  background: #1d1f22;
  border: 1px solid #35383e;
  padding: 8px 10px;
  border-radius: 10px;
  box-shadow: 0px 4px 18px rgba(0, 0, 0, 0.4);
  pointer-events: none;
  min-width: 130px;
}
</style>
