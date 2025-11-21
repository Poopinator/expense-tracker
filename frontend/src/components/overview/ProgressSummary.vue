<template>
  <div
    class="panel p-6 rounded-2xl bg-[#1D1F22]/80 border border-[#35383E] shadow-lg backdrop-blur-xl"
  >
    <h2 class="text-lg font-semibold text-white mb-4">
      📈 Spending Summary
    </h2>

    <!-- Range Picker -->
    <div class="flex flex-wrap gap-3 mb-5">
      <select v-model="localRange" class="select-graphite">
        <option value="today">Today</option>
        <option value="7">Last 7 Days</option>
        <option value="30">Last 30 Days</option>
        <option value="custom">Custom Range…</option>
      </select>

      <template v-if="localRange === 'custom'">
        <input type="date" v-model="customFrom" class="select-graphite" />
        <input type="date" v-model="customTo" class="select-graphite" />
      </template>

      <button @click="applyRange" class="btn-graphite px-4 py-2">
        Apply
      </button>
    </div>

    <!-- Main Numbers -->
    <div class="grid grid-cols-3 gap-4 text-center mb-6">
      <div class="card-mini">
        <p class="text-xs text-gray-400">Today</p>
        <p class="text-xl font-bold text-indigo-400">${{ fmtAmt(daily.today) }}</p>
      </div>

      <div class="card-mini">
        <p class="text-xs text-gray-400">This Week</p>
        <p class="text-xl font-bold text-indigo-400">${{ fmtAmt(daily.week) }}</p>
      </div>

      <div class="card-mini">
        <p class="text-xs text-gray-400">This Month</p>
        <p class="text-xl font-bold text-indigo-400">${{ fmtAmt(daily.month) }}</p>
      </div>
    </div>

    <!-- Bar Chart -->
    <div class="h-[160px] relative mb-6">
      <canvas ref="barRef"></canvas>
    </div>

    <!-- Top Category -->
    <div
      v-if="topCategory"
      class="p-4 rounded-xl bg-[#26292E]/70 border border-[#35383E]"
    >
      <p class="text-gray-400 text-sm">Most Spent Category</p>
      <p class="font-semibold text-white text-lg mt-1">{{ topCategory.name }}</p>
      <p class="text-indigo-400 text-sm">${{ fmtAmt(topCategory.value) }}</p>
    </div>

    <p v-else class="text-gray-500 text-sm text-center">
      No expenses in this period.
    </p>
  </div>
</template>

<script setup>
import { ref, watch, computed, onMounted, nextTick } from "vue";
import Chart from "chart.js/auto";
import api from "../../api";

const props = defineProps({
  expenses: {
    type: Array,
    required: true,
  },
});

// Range state
const localRange = ref("7");
const customFrom = ref("");
const customTo = ref("");

// Computed — selected range filter
const filtered = computed(() => {
  if (!props.expenses.length) return [];

  const now = new Date();
  let fromDate;

  if (localRange.value === "today") {
    fromDate = new Date(now.toDateString());
    return props.expenses.filter(
      (e) => new Date(e.date).toDateString() === fromDate.toDateString()
    );
  }

  if (localRange.value === "7" || localRange.value === "30") {
    fromDate = new Date();
    fromDate.setDate(fromDate.getDate() - Number(localRange.value));
    return props.expenses.filter((e) => new Date(e.date) >= fromDate);
  }

  if (localRange.value === "custom" && customFrom.value && customTo.value) {
    const start = new Date(customFrom.value + "T00:00:00");
    const end = new Date(customTo.value + "T23:59:59");
    return props.expenses.filter((e) => {
      const d = new Date(e.date);
      return d >= start && d <= end;
    });
  }

  return props.expenses;
});

// Today / Week / Month metrics
const daily = computed(() => {
  const today = new Date().toDateString();

  const weekStart = new Date();
  weekStart.setDate(weekStart.getDate() - 7);

  const month = new Date().getMonth();
  const year = new Date().getFullYear();

  let t = 0,
    w = 0,
    m = 0;

  for (const e of props.expenses) {
    const d = new Date(e.date);
    const amt = Number(e.amount);

    if (d.toDateString() === today) t += amt;
    if (d >= weekStart) w += amt;
    if (d.getMonth() === month && d.getFullYear() === year) m += amt;
  }

  return { today: t, week: w, month: m };
});

// Top category inside range
const topCategory = computed(() => {
  const map = {};
  for (const e of filtered.value) {
    map[e.category] = (map[e.category] || 0) + Number(e.amount);
  }
  let best = null;
  for (const [k, v] of Object.entries(map)) {
    if (!best || v > best.value) best = { name: k, value: v };
  }
  return best;
});

// Chart
const barRef = ref(null);
let barChart;

function drawChart() {
  if (!barRef.value) return;

  if (barChart) {
    barChart.destroy();
    barChart = null;
  }

  const data = [daily.value.today, daily.value.week, daily.value.month];

  barChart = new Chart(barRef.value, {
    type: "bar",
    data: {
      labels: ["Today", "Week", "Month"],
      datasets: [
        {
          label: "Spending",
          data,
          backgroundColor: [
            "rgba(129,140,248,0.7)",
            "rgba(236,72,153,0.7)",
            "rgba(59,130,246,0.7)",
          ],
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        x: { ticks: { color: "#e5e7eb" } },
        y: { ticks: { color: "#e5e7eb" } },
      },
    },
  });
}

watch(daily, () => nextTick(drawChart));
watch(filtered, () => nextTick(drawChart));

onMounted(() => {
  nextTick(drawChart);
});

// Range apply
function applyRange() {
  nextTick(drawChart);
}

const fmtAmt = (n) => Number(n || 0).toFixed(2);
</script>

<style scoped>
.card-mini {
  @apply bg-[#26292E]/70 border border-[#35383E] rounded-xl p-3 shadow-sm;
}

.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E]
  focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.btn-graphite {
  @apply bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow px-4 py-2 transition;
}
</style>
