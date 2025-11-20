<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 overflow-hidden">
    <!-- Particle Background -->
    <div class="particle-bg"></div>

    <!-- Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <div class="relative z-10 mx-auto max-w-7xl px-6 py-10">

      <!-- 🔥 Header -->
      <header class="mb-10 text-center animate-fade-in">
        <h1
          class="text-4xl md:text-5xl font-extrabold tracking-tight bg-gradient-to-r from-indigo-400 to-pink-400 text-transparent bg-clip-text"
        >
          📊 Insights & Reports
        </h1>
        <p class="text-gray-400 mt-3 text-lg">
          Your financial trends, spending patterns & budget performance — all in one place.
        </p>
      </header>

      <!-- 🔍 Filters -->
      <section class="flex flex-wrap items-center gap-3 mb-8 justify-center">
        <button
          v-for="p in presets"
          :key="p.label"
          @click="setPreset(p)"
          :class="presetClass(p.label)"
        >
          {{ p.label }}
        </button>

        <input type="date" v-model="from" class="select-graphite" />
        <input type="date" v-model="to" class="select-graphite" />
        <button class="btn-graphite" @click="refresh">Apply</button>
      </section>

      <!-- ⚠ Empty State -->
      <div v-if="!expenses.length && !loading" class="text-center py-20">
        <p class="text-gray-400 text-xl">No expenses found for this period.</p>
        <p class="text-gray-500 text-sm mt-2">Try expanding the date range above.</p>
      </div>

      <!-- ⏳ Skeleton Loader -->
      <div v-if="loading" class="grid md:grid-cols-3 gap-5 mb-8 animate-pulse">
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
      </div>

      <!-- 📌 Primary Insight Cards -->
      <div v-if="!loading" class="grid md:grid-cols-3 gap-5 mb-6 animate-slide-up">
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Total Spent (Current Period)</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            ${{ fmt(totalSpent) }}
          </p>
          <p class="text-xs text-gray-500 mt-1">{{ rangeLabel }}</p>
        </div>

        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Top Category</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            {{ topCategory?.name || "—" }}
          </p>
          <p v-if="topCategory" class="text-xs text-gray-500 mt-1">
            ${{ fmt(topCategory.value) }} spent
          </p>
        </div>

        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Most Active Day</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            {{ busiestDay || "—" }}
          </p>
        </div>
      </div>

      <!-- 📈 Advanced Comparison Cards -->
      <div
        v-if="!loading && expenses.length"
        class="grid md:grid-cols-3 gap-5 mb-10 animate-slide-up"
      >
        <!-- vs Previous Period -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Vs Previous Period</h3>
          <p
            class="text-2xl font-bold mt-1"
            :class="deltaAbs > 0 ? 'text-rose-400' : deltaAbs < 0 ? 'text-emerald-400' : 'text-gray-300'"
          >
            <span v-if="hasPrevData">
              {{ deltaAbs > 0 ? "▲" : deltaAbs < 0 ? "▼" : "•" }}
              {{ deltaPctText }}
            </span>
            <span v-else>—</span>
          </p>
          <p class="text-xs text-gray-500 mt-1">
            Prev: ${{ fmt(totalPrevSpent) }}
          </p>
        </div>

        <!-- Daily Average -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Daily Average</h3>
          <p class="text-2xl font-bold text-indigo-400 mt-1">
            ${{ fmt(avgPerDay) }}
          </p>
          <p class="text-xs text-gray-500 mt-1">
            Prev: ${{ fmt(avgPrevPerDay) }} per day
          </p>
        </div>

        <!-- Biggest Category Jump -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Biggest Category Jump</h3>
          <p class="text-2xl font-bold text-indigo-400 mt-1">
            {{ biggestIncreaseCategory?.name || "—" }}
          </p>
          <p v-if="biggestIncreaseCategory" class="text-xs text-gray-500 mt-1">
            +${{ fmt(biggestIncreaseCategory.diff) }} vs prev period
          </p>
        </div>
      </div>

      <!-- 📊 Charts -->
      <section v-if="expenses.length" class="grid md:grid-cols-2 gap-8">
        <!-- Trend Chart -->
        <div class="panel animate-fade-in">
          <h3 class="panel-title">Spending Trend</h3>
          <div class="chart-wrap">
            <canvas ref="trendRef"></canvas>
          </div>
          <button class="export-btn" @click="exportChart(trendRef)">Download PNG</button>
        </div>

        <!-- Donut Chart -->
        <div class="panel animate-fade-in">
          <h3 class="panel-title">Category Breakdown</h3>
          <div class="chart-wrap">
            <canvas ref="donutRef"></canvas>
          </div>
          <button class="export-btn" @click="exportChart(donutRef)">Download PNG</button>
        </div>
      </section>

      <!-- 🧠 Advanced Insights Panel -->
      <section
        v-if="!loading && expenses.length && smartInsights.length"
        class="panel mt-10 animate-slide-up"
      >
        <h3 class="panel-title">Advanced Insights</h3>
        <ul class="space-y-2 text-sm text-gray-300">
          <li
            v-for="(ins, idx) in smartInsights"
            :key="idx"
            class="flex items-start gap-2"
          >
            <span class="text-indigo-400 mt-0.5">•</span>
            <span>{{ ins }}</span>
          </li>
        </ul>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import Chart from "chart.js/auto";
import api from "../api";

const rawExpenses = ref([]);
const expenses = ref([]);
const loading = ref(true);

const today = new Date();
const to = ref(new Date().toISOString().slice(0, 10));
const from = ref(
  new Date(today.getFullYear(), today.getMonth(), 1).toISOString().slice(0, 10)
);

const presets = [
  { label: "This Month", days: "month" },
  { label: "Last 30d", days: 30 },
  { label: "Last 90d", days: 90 },
  { label: "Year to Date", days: "ytd" },
];

const activePreset = ref("This Month");

function presetClass(label) {
  return [
    "px-4 py-2 rounded-lg border transition",
    activePreset.value === label
      ? "bg-indigo-600 text-white border-indigo-500 shadow"
      : "bg-[#1D1F22] text-gray-300 border-[#35383E] hover:text-white",
  ].join(" ");
}

function setPreset(p) {
  activePreset.value = p.label;
  applyPreset(p.days);
}

function applyPreset(kind) {
  const now = new Date();

  if (kind === "month") {
    from.value = new Date(now.getFullYear(), now.getMonth(), 1)
      .toISOString()
      .slice(0, 10);
    to.value = now.toISOString().slice(0, 10);
  } else if (kind === "ytd") {
    from.value = new Date(now.getFullYear(), 0, 1).toISOString().slice(0, 10);
    to.value = now.toISOString().slice(0, 10);
  } else {
    const start = new Date(now);
    start.setDate(start.getDate() - kind);
    from.value = start.toISOString().slice(0, 10);
    to.value = now.toISOString().slice(0, 10);
  }

  refresh();
}

async function refresh() {
  loading.value = true;

  const { data } = await api.get("/api/expense");
  rawExpenses.value = data || [];

  const f = new Date(from.value + "T00:00:00");
  const t = new Date(to.value + "T23:59:59");

  expenses.value = rawExpenses.value.filter((e) => {
    const d = new Date(e.date);
    return d >= f && d <= t;
  });

  loading.value = false;
  drawCharts();
}

// ===== Range + Previous Period Helpers =====
const rangeInfo = computed(() => {
  const start = new Date(from.value + "T00:00:00");
  const end = new Date(to.value + "T23:59:59");
  const msPerDay = 1000 * 60 * 60 * 24;
  const lengthDays = Math.max(
    1,
    Math.round((end.getTime() - start.getTime()) / msPerDay) + 1
  );

  const prevEnd = new Date(start.getTime() - msPerDay);
  const prevStart = new Date(
    prevEnd.getTime() - msPerDay * (lengthDays - 1)
  );

  return { start, end, prevStart, prevEnd, lengthDays };
});

const rangeLabel = computed(() => {
  const { start, end } = rangeInfo.value;
  return `${start.toLocaleDateString()} → ${end.toLocaleDateString()}`;
});

function getRangeExpenses(start, end) {
  return rawExpenses.value.filter((e) => {
    const d = new Date(e.date);
    return d >= start && d <= end;
  });
}

const prevExpenses = computed(() => {
  const { prevStart, prevEnd } = rangeInfo.value;
  return getRangeExpenses(prevStart, prevEnd);
});

// ===== Core Metrics =====
const totalSpent = computed(() =>
  expenses.value.reduce((s, e) => s + Number(e.amount || 0), 0)
);

const totalPrevSpent = computed(() =>
  prevExpenses.value.reduce((s, e) => s + Number(e.amount || 0), 0)
);

const deltaAbs = computed(() => totalSpent.value - totalPrevSpent.value);

const hasPrevData = computed(() => totalPrevSpent.value > 0);

const deltaPct = computed(() => {
  if (!hasPrevData.value) return null;
  return (deltaAbs.value / totalPrevSpent.value) * 100;
});

const deltaPctText = computed(() => {
  if (deltaPct.value === null) return "No previous data";
  const abs = Math.abs(deltaPct.value).toFixed(1);
  if (deltaAbs.value > 0) return `Up ${abs}%`;
  if (deltaAbs.value < 0) return `Down ${abs}%`;
  return "Flat vs previous period";
});

// Daily averages
const avgPerDay = computed(() => {
  const { lengthDays } = rangeInfo.value;
  return lengthDays ? totalSpent.value / lengthDays : 0;
});

const avgPrevPerDay = computed(() => {
  const { lengthDays } = rangeInfo.value;
  return lengthDays ? totalPrevSpent.value / lengthDays : 0;
});

// Top category (current period)
const topCategory = computed(() => {
  const byCat = {};
  for (const e of expenses.value) {
    byCat[e.category] = (byCat[e.category] || 0) + Number(e.amount || 0);
  }
  let best = null;
  for (const [name, val] of Object.entries(byCat)) {
    if (!best || val > best.value) best = { name, value: val };
  }
  return best;
});

// Busiest day (current period)
const busiestDay = computed(() => {
  const byDay = {};
  for (const e of expenses.value) {
    const k = new Date(e.date).toLocaleDateString();
    byDay[k] = (byDay[k] || 0) + Number(e.amount || 0);
  }
  let best = null;
  for (const [k, v] of Object.entries(byDay)) {
    if (!best || v > best.value) best = { day: k, value: v };
  }
  return best?.day;
});

// Biggest category jump vs previous period
function buildCatMap(list) {
  const map = {};
  for (const e of list) {
    map[e.category] = (map[e.category] || 0) + Number(e.amount || 0);
  }
  return map;
}

const biggestIncreaseCategory = computed(() => {
  const curr = buildCatMap(expenses.value);
  const prev = buildCatMap(prevExpenses.value);

  let best = null;
  for (const [name, value] of Object.entries(curr)) {
    const diff = value - (prev[name] || 0);
    if (diff > 0 && (!best || diff > best.diff)) {
      best = { name, diff };
    }
  }
  return best;
});

// 🧠 Smart Insights text list
const smartInsights = computed(() => {
  const list = [];

  if (!expenses.value.length) return list;

  if (deltaPct.value !== null) {
    if (deltaAbs.value > 0) {
      list.push(
        `You spent ${deltaPctText.value} more than the previous period (+$${fmt(
          deltaAbs.value
        )}).`
      );
    } else if (deltaAbs.value < 0) {
      list.push(
        `Nice! Spending is ${deltaPctText.value} vs the previous period (-$${fmt(
          Math.abs(deltaAbs.value)
        )}).`
      );
    }
  }

  if (topCategory.value) {
    list.push(
      `Your highest spend category is "${topCategory.value.name}" with $${fmt(
        topCategory.value.value
      )} during this period.`
    );
  }

  if (biggestIncreaseCategory.value) {
    list.push(
      `"${biggestIncreaseCategory.value.name}" increased the most vs last period (+$${fmt(
        biggestIncreaseCategory.value.diff
      )}).`
    );
  }

  const { lengthDays } = rangeInfo.value;
  if (lengthDays >= 7) {
    list.push(
      `On average, you’re spending $${fmt(avgPerDay.value)} per day in this range.`
    );
  }

  return list;
});

// ===== Charts =====
const trendRef = ref(null);
const donutRef = ref(null);

let trendChart, donutChart;

function destroyCharts() {
  if (trendChart) trendChart.destroy();
  if (donutChart) donutChart.destroy();
}

function drawCharts() {
  destroyCharts();
  if (!expenses.value.length) return;

  // Trend: sum by date
  const dayMap = {};
  const sorted = [...expenses.value].sort(
    (a, b) => new Date(a.date) - new Date(b.date)
  );
  for (const e of sorted) {
    const key = new Date(e.date).toLocaleDateString();
    dayMap[key] = (dayMap[key] || 0) + Number(e.amount || 0);
  }

  if (trendRef.value) {
    trendChart = new Chart(trendRef.value, {
      type: "line",
      data: {
        labels: Object.keys(dayMap),
        datasets: [
          {
            data: Object.values(dayMap),
            borderColor: "rgba(129,140,248,0.9)",
            backgroundColor: "rgba(129,140,248,0.12)",
            tension: 0.4,
            fill: true,
            pointRadius: 3,
          },
        ],
      },
      options: {
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          x: { ticks: { color: "#9ca3af" }, grid: { color: "rgba(255,255,255,0.06)" } },
          y: { ticks: { color: "#9ca3af" }, grid: { color: "rgba(255,255,255,0.06)" } },
        },
      },
    });
  }

  // Donut: sum by category
  const catMap = {};
  for (const e of expenses.value) {
    catMap[e.category] = (catMap[e.category] || 0) + Number(e.amount || 0);
  }

  const colors = [
    "#818CF8",
    "#EC4899",
    "#F59E0B",
    "#10B981",
    "#3B82F6",
    "#8B5CF6",
  ];

  if (donutRef.value) {
    donutChart = new Chart(donutRef.value, {
      type: "doughnut",
      data: {
        labels: Object.keys(catMap),
        datasets: [
          {
            data: Object.values(catMap),
            backgroundColor: colors,
          },
        ],
      },
      options: {
        plugins: { legend: { labels: { color: "#d1d5db" } } },
      },
    });
  }
}

// Export PNG
function exportChart(refEl) {
  if (!refEl.value) return;
  const url = refEl.value.toDataURL("image/png");
  const link = document.createElement("a");
  link.download = "chart.png";
  link.href = url;
  link.click();
}

const fmt = (n) => Number(n || 0).toFixed(2);

onMounted(refresh);
</script>

<style scoped>
/* Skeleton */
.skeleton-card {
  @apply h-24 rounded-2xl bg-gray-700/20 border border-gray-700;
}

/* Animations */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
  }
}
.animate-fade-in {
  animation: fadeIn 0.6s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
  }
}
.animate-slide-up {
  animation: slideUp 0.7s ease-out;
}

/* Core UI */
.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200
         border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.btn-graphite {
  @apply px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold
         rounded-lg shadow-md hover:shadow-indigo-500/30 transition;
}

.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 shadow-lg
         backdrop-blur-xl transition hover:shadow-indigo-500/20;
}

.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6
         shadow-lg backdrop-blur-xl;
}

.panel-title {
  @apply text-white font-semibold text-lg mb-4;
}

.chart-wrap {
  @apply relative w-full h-[330px];
}

/* Export Button */
.export-btn {
  @apply mt-4 text-xs text-gray-400 underline hover:text-indigo-400 transition;
}
</style>
