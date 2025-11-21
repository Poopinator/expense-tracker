<template>
  <div class="mini-chart-panel">
    <div class="header">
      <h3 class="title">Last 7 Days</h3>
    </div>

    <div class="chart-container">
      <canvas ref="chartRef"></canvas>
    </div>

    <p class="sub">
      Daily spending trend for the past week.
    </p>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch } from "vue";
import Chart from "chart.js/auto";

// props: full expense list from Dashboard
const props = defineProps({
  expenses: { type: Array, required: true } // [{date, amount}]
});

const chartRef = ref(null);
let chartInstance = null;

// Build 7 day list
function compute7Days() {
  const today = new Date();
  const days = [];

  for (let i = 6; i >= 0; i--) {
    const d = new Date();
    d.setDate(today.getDate() - i);
    const key = d.toISOString().slice(0, 10);
    const amount = props.expenses
      .filter(e => e.date.startsWith(key))
      .reduce((s, e) => s + Number(e.amount), 0);

    days.push({
      label: d.toLocaleDateString("default", { weekday: "short" }),
      amount
    });
  }

  return days;
}

// Draw chart
function renderChart() {
  if (!chartRef.value) return;

  if (chartInstance) chartInstance.destroy();

  const data = compute7Days();

  chartInstance = new Chart(chartRef.value, {
    type: "bar",
    data: {
      labels: data.map(d => d.label),
      datasets: [
        {
          data: data.map(d => d.amount),
          backgroundColor: "rgba(129,140,248,0.8)",
          borderRadius: 6
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        x: {
          ticks: { color: "#d1d5db" },
          grid: { display: false }
        },
        y: {
          ticks: { color: "#9ca3af" },
          grid: { color: "rgba(255,255,255,0.07)" },
          suggestedMax: Math.max(...data.map(d => d.amount), 20)
        }
      }
    }
  });
}

onMounted(renderChart);

watch(() => props.expenses, renderChart, { deep: true });

onBeforeUnmount(() => {
  if (chartInstance) chartInstance.destroy();
});
</script>

<style scoped>
.mini-chart-panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 shadow-lg backdrop-blur-xl;
}

.header {
  @apply flex justify-between items-center mb-3;
}

.title {
  @apply text-white font-semibold text-lg;
}

.chart-container {
  @apply w-full h-[180px];
}

.sub {
  @apply text-gray-400 text-xs text-center mt-3;
}
</style>


