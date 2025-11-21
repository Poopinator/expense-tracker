<template>
  <div class="panel">
    <h2 class="panel-title">📅 Spending Calendar</h2>

    <p class="text-gray-400 text-xs mb-4">
      Monthly calendar showing what you spent each day.
    </p>

    <!-- Empty -->
    <div v-if="months.length === 0" class="text-gray-500 text-center py-6">
      No spending data in this range.
    </div>

    <!-- month blocks -->
    <div v-else class="space-y-10">
      <div v-for="m in months" :key="m.key" class="month-card">
        <!-- month header -->
        <div class="flex items-center justify-between mb-3">
          <h3 class="text-gray-200 font-semibold">
            {{ monthNames[m.month] }} {{ m.year }}
          </h3>
          <span class="text-xs text-gray-400">
            Total: ${{ fmt(m.total) }}
          </span>
        </div>

        <!-- weekday labels -->
        <div class="weekday-row">
          <span v-for="d in weekdays" :key="d" class="weekday-label">{{ d }}</span>
        </div>

        <!-- calendar grid -->
        <div class="calendar-grid">
          <div
            v-for="(cell, i) in m.grid"
            :key="i"
            class="day-box"
            @mouseenter="showTooltip(cell, $event)"
            @mouseleave="hideTooltip"
          >
            <div v-if="cell.date" class="inner-day">
              <span class="day-num">{{ cell.day }}</span>
              <span v-if="cell.amount > 0" class="day-amt">
                ${{ cell.amount }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- tooltip -->
    <div
      v-if="tooltip.visible"
      class="tooltip-box"
      :style="{ top: tooltip.y + 'px', left: tooltip.x + 'px' }"
    >
      <p class="font-semibold text-white text-xs">{{ tooltip.date }}</p>
      <p class="text-indigo-300 text-[11px] mt-1">
        ${{ fmt(tooltip.amount) }} spent
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  expenses: {
    type: Array,
    default: () => []
  }
});

const monthNames = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];
const weekdays = ["Sun","Mon","Tue","Wed","Thu","Fri","Sat"];

const months = ref([]);

/* Build monthly calendar */
function buildCalendar() {
  const exp = props.expenses || [];

  if (!exp.length) {
    months.value = [];
    return;
  }

  // map date -> amount
  const map = {};
  for (const e of exp) {
    if (!e.date) continue;
    const key = new Date(e.date).toISOString().slice(0, 10);
    map[key] = (map[key] || 0) + Number(e.amount || 0);
  }

  // get min/max date
  const dates = exp.map(e => new Date(e.date)).sort((a, b) => a - b);
  const start = dates[0];
  const end = dates[dates.length - 1];

  const result = [];

  let cursor = new Date(start.getFullYear(), start.getMonth(), 1);

  while (cursor <= end) {
    const year = cursor.getFullYear();
    const month = cursor.getMonth();

    const first = new Date(year, month, 1);
    const last = new Date(year, month + 1, 0);

    const grid = [];
    const startOffset = first.getDay();

    // Fill empty cells
    for (let i = 0; i < startOffset; i++) {
      grid.push({ date: null });
    }

    // Fill actual days
    for (let d = 1; d <= last.getDate(); d++) {
      const dayObj = new Date(year, month, d);
      const key = dayObj.toISOString().slice(0, 10);

      grid.push({
        date: key,
        day: d,
        amount: map[key] || 0
      });
    }

    // Month total
    const total = Object.entries(map)
      .filter(([k]) => k.startsWith(`${year}-${String(month+1).padStart(2,"0")}`))
      .reduce((s, [,v]) => s + v, 0);

    result.push({
      key: `${year}-${month}`,
      year,
      month,
      grid,
      total
    });

    cursor = new Date(year, month + 1, 1);
  }

  months.value = result;
}

/* Tooltip */
const tooltip = ref({
  visible: false,
  date: "",
  amount: 0,
  x: 0,
  y: 0
});

function showTooltip(cell, evt) {
  if (!cell.date) return;

  const rect = evt.target.getBoundingClientRect();

  tooltip.value = {
    visible: true,
    date: new Date(cell.date).toLocaleDateString(),
    amount: cell.amount,
    x: rect.right + 10 + window.scrollX,
    y: rect.top + window.scrollY
  };
}

function hideTooltip() {
  tooltip.value.visible = false;
}

const fmt = n => Number(n || 0).toFixed(2);

watch(() => props.expenses, buildCalendar, { deep: true, immediate: true });
</script>

<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E]
         rounded-2xl p-6 shadow-xl mb-6;
}

.panel-title {
  @apply text-white font-semibold mb-2;
}

.month-card {
  @apply bg-[#16181C]/80 border border-[#2a2d31] rounded-xl p-4;
}

/* Weekday labels */
.weekday-row {
  @apply grid grid-cols-7 text-center text-[11px] text-gray-500 mb-2;
}

/* Calendar grid */
.calendar-grid {
  @apply grid grid-cols-7 gap-2;
}

/* Day cells */
.day-box {
  @apply w-full h-16 bg-[#1F2126] rounded-lg border border-[#2a2d31]
         flex items-center justify-center;
}

.inner-day {
  @apply flex flex-col items-center;
}

.day-num {
  @apply text-gray-300 text-xs font-medium;
}

.day-amt {
  @apply text-indigo-400 text-xs font-semibold mt-1;
}

/* Tooltip */
.tooltip-box {
  position: absolute;
  z-index: 9999;
  background: #111827;
  border: 1px solid #4b5563;
  padding: 6px 9px;
  border-radius: 8px;
  box-shadow: 0px 6px 20px rgba(0,0,0,0.55);
  animation: fadeIn 0.15s ease-out forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(4px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
