<template>
  <div class="calendar-panel">

    <!-- Header -->
    <div class="calendar-header">
      <button @click="prevMonth" class="nav-btn">‹</button>

      <h2 class="month-title">
        {{ monthName(displayYear, displayMonth) }}
      </h2>

      <button @click="nextMonth" class="nav-btn">›</button>
    </div>

    <!-- Weekday names -->
    <div class="week-row">
      <div v-for="d in weekDays" :key="d" class="week-day">
        {{ d }}
      </div>
    </div>

    <!-- Calendar grid -->
    <div class="grid">
      <div
        v-for="(day, index) in calendarDays"
        :key="index"
        class="day-cell"
        :class="{ 'inactive': !day.inCurrentMonth }"
        :style="{
          backgroundColor: heatColor(day.total),
        }"
      >
        <span class="day-number">{{ day.date.getDate() }}</span>

        <span v-if="day.total > 0" class="amount">
          ${{ day.total.toFixed(0) }}
        </span>
      </div>
    </div>

  </div>
</template>

<script setup>
import { computed, ref, watch } from "vue";

const props = defineProps({
  expenses: { type: Array, required: true }, // [{date, amount}, ...]
  startDate: { type: String, required: true },
  endDate: { type: String, required: true }
});

// =======================
// Calendar month control
// =======================
const base = new Date(props.endDate); // show month of "to" date
const displayMonth = ref(base.getMonth());
const displayYear = ref(base.getFullYear());

// =======================
// Month navigation
// =======================
function prevMonth() {
  if (displayMonth.value === 0) {
    displayMonth.value = 11;
    displayYear.value--;
  } else displayMonth.value--;
}

function nextMonth() {
  if (displayMonth.value === 11) {
    displayMonth.value = 0;
    displayYear.value++;
  } else displayMonth.value++;
}

// =======================
// Build calendar grid
// =======================
const weekDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

const calendarDays = computed(() => {
  const year = displayYear.value;
  const month = displayMonth.value;

  const first = new Date(year, month, 1);
  const startDay = first.getDay();

  const daysInMonth = new Date(year, month + 1, 0).getDate();

  // create full 42 cells grid
  const cells = [];

  // previous month's trailing days
  for (let i = 0; i < startDay; i++) {
    const d = new Date(year, month, i - startDay + 1);
    cells.push({ date: d, total: 0, inCurrentMonth: false });
  }

  // current month
  for (let day = 1; day <= daysInMonth; day++) {
    const d = new Date(year, month, day);
    const total = calcTotalForDay(d);
    cells.push({ date: d, total, inCurrentMonth: true });
  }

  // next month filler
  while (cells.length % 7 !== 0) {
    const d = new Date(year, month + 1, cells.length - (startDay + daysInMonth) + 1);
    cells.push({ date: d, total: 0, inCurrentMonth: false });
  }

  return cells;
});

// =======================
// Helpers
// =======================
function calcTotalForDay(day) {
  const dayStr = day.toISOString().slice(0, 10);
  return props.expenses
    .filter(e => e.date.startsWith(dayStr))
    .reduce((s, e) => s + Number(e.amount), 0);
}

function monthName(year, month) {
  return new Date(year, month).toLocaleString("default", {
    month: "long",
    year: "numeric"
  });
}

function heatColor(val) {
  if (val <= 0) return "transparent";

  // min/max color intensity
  const max = 80;
  const level = Math.min(val / max, 1);
  const alpha = 0.15 + level * 0.65;

  return `rgba(129,140,248,${alpha})`; // indigo heat
}
</script>

<style scoped>
.calendar-panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 backdrop-blur-xl shadow-lg;
}

.calendar-header {
  @apply flex items-center justify-between mb-4;
}

.month-title {
  @apply text-lg font-semibold text-gray-200;
}

.nav-btn {
  @apply w-8 h-8 flex items-center justify-center rounded-lg bg-[#26292E] text-gray-300 
         hover:bg-indigo-600 hover:text-white transition;
}

.week-row {
  @apply grid grid-cols-7 mb-1;
}

.week-day {
  @apply text-center text-xs text-gray-400;
}

.grid {
  @apply grid grid-cols-7 gap-1;
}

.day-cell {
  @apply relative rounded-lg p-2 text-gray-100 text-xs flex flex-col items-center transition cursor-default;
}

.day-cell.inactive {
  @apply opacity-40;
}

.day-number {
  @apply font-semibold;
}

.amount {
  @apply text-[10px] text-indigo-300 mt-1;
}
</style>

