<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 overflow-hidden">
    <!-- Particle Background -->
    <div class="particle-bg"></div>

    <!-- Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <div class="relative z-10 mx-auto max-w-7xl px-6 py-10">
      <!-- 🔥 Header -->
      <header class="mb-10 text-center animate-fade-in">
        <div class="inline-flex items-center justify-center mb-3">
          <span class="text-4xl mr-2 inline-block leading-none" aria-hidden="true">
            📊
          </span>
          <span
            class="text-4xl md:text-5xl font-extrabold tracking-tight bg-gradient-to-r from-indigo-400 to-pink-400 text-transparent bg-clip-text">
            Insights &amp; Reports
          </span>
        </div>
        <p class="text-gray-400 mt-3 text-lg max-w-2xl mx-auto">
          Your financial trends, spending patterns, forecasts &amp; budget performance — all in one place.
        </p>
      </header>

      <!-- 🔍 Filters & Export -->
      <section class="flex flex-wrap items-center gap-3 mb-8 justify-center animate-slide-up">
        <button v-for="p in presets" :key="p.label" @click="setPreset(p)" :class="presetClass(p.label)">
          {{ p.label }}
        </button>

        <input type="date" v-model="from" class="select-graphite" />
        <input type="date" v-model="to" class="select-graphite" />

        <button class="btn-graphite" @click="refresh">Apply</button>

        <div class="flex flex-wrap gap-2 ml-3">
          <button
            class="px-3 py-2 text-xs rounded-lg bg-[#1D1F22] border border-[#35383E] text-gray-300 hover:text-white hover:border-indigo-500 transition"
            @click="exportCsv">
            ⬇ Export CSV
          </button>
          <button
            class="px-3 py-2 text-xs rounded-lg bg-[#1D1F22] border border-[#35383E] text-gray-300 hover:text-white hover:border-indigo-500 transition"
            @click="exportPdf">
            ⬇ Export PDF
          </button>
        </div>
      </section>

      <!-- ⚠ Empty State -->
      <div v-if="!expenses.length && !loading" class="text-center py-20">
        <p class="text-gray-400 text-xl">No expenses found for this period.</p>
        <p class="text-gray-500 text-sm mt-2">
          Try expanding the date range above.
        </p>
      </div>

      <!-- ⏳ Skeleton Loader -->
      <div v-if="loading" class="grid md:grid-cols-3 gap-5 mb-8 animate-pulse">
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
      </div>

      <!-- 📌 Primary Insight Cards -->
      <div v-if="!loading" class="grid md:grid-cols-3 gap-5 mb-6 animate-slide-up">
        <!-- Total Spent -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Total Spent (Current Period)</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            ${{ fmt(totalSpent) }}
          </p>
          <p class="text-xs text-gray-500 mt-1">{{ rangeLabel }}</p>
        </div>

        <!-- Top Category -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Top Category</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            {{ topCategory?.name || "—" }}
          </p>
          <p v-if="topCategory" class="text-xs text-gray-500 mt-1">
            ${{ fmt(topCategory.value) }} spent
          </p>
        </div>

        <!-- Most Active Day -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Most Active Day</h3>
          <p class="text-3xl font-bold text-indigo-400 mt-1">
            {{ busiestDay || "—" }}
          </p>
        </div>
      </div>

      <!-- 📈 Advanced Metrics -->
      <div v-if="!loading && expenses.length" class="grid md:grid-cols-4 gap-5 mb-10 animate-slide-up">
        <!-- Vs Previous Period -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">Vs Previous Period</h3>
          <p class="text-2xl font-bold mt-1" :class="deltaAbs > 0
              ? 'text-rose-400'
              : deltaAbs < 0
                ? 'text-emerald-400'
                : 'text-gray-300'
            ">
            <span v-if="hasPrevData">
              {{ deltaAbs > 0 ? "▲" : deltaAbs < 0 ? "▼" : "•" }} {{ deltaPctText }} </span>
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
            Prev: ${{ fmt(avgPrevPerDay) }} / day
          </p>
        </div>

        <!-- 30d Spend Forecast -->
        <div class="card-graphite text-center">
          <h3 class="text-gray-400 text-sm">30-Day Spend Forecast</h3>
          <p class="text-2xl font-bold text-indigo-400 mt-1">
            ${{ fmt(forecastNext30) }}
          </p>
          <p class="text-xs text-gray-500 mt-1">
            vs current period: {{ forecastVsCurrentText }}
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

      <!-- SECTION 2: 📈 Big Trend + Forecast (full width, A1) -->
      <section v-if="expenses.length" class="mb-10">
        <div class="panel animate-fade-in">
          <h3 class="panel-title">
            Spending Trend &amp; 30-Day Forecast
          </h3>
          <div class="chart-wrap chart-wrap--tall">
            <canvas ref="trendRef"></canvas>
          </div>
          <p class="mt-3 text-xs text-gray-500">
            Solid line = actual spending in selected range. Dashed line = hybrid
            forecast for the next 30 days.
          </p>
          <button class="export-btn" @click="exportChartPng(trendRef)">
            Download trend as PNG
          </button>
        </div>
      </section>

      <!-- SECTION 3: 📊 Category Breakdown + Analytics (bottom row) -->
      <section v-if="expenses.length" class="grid md:grid-cols-2 gap-8">
        <!-- Donut: Category Breakdown -->
        <div class="panel animate-fade-in">
          <h3 class="panel-title">Category Breakdown</h3>

          <!-- Add donut-center class here -->
          <div class="chart-wrap donut-center">
            <canvas ref="donutRef"></canvas>
          </div>

          <button class="export-btn" @click="exportChartPng(donutRef)">
            Download breakdown as PNG
          </button>
        </div>


        <!-- Category Analytics Panel (B3 + C1 style) -->
        <div class="panel animate-fade-in">
          <h3 class="panel-title flex items-center justify-between">
            <span>Category Analytics</span>
            <span v-if="categoryStats.length" class="text-xs text-gray-400">
              {{ categoryStats.length }} categories
            </span>
          </h3>

          <div v-if="!categoryStats.length" class="text-sm text-gray-500 py-4">
            No category data available for this range.
          </div>

          <div v-else class="space-y-3">
            <!-- Quick summary -->
            <div class="text-xs text-gray-400">
              <div>
                Top category:
                <span class="text-indigo-300 font-medium">
                  {{ categoryStats[0].name }}
                </span>
              </div>
              <div class="mt-1">
                Top 3 categories account for
                <span class="font-medium">
                  {{ top3Share.toFixed(1) }}%
                </span>
                of your spending in this range.
              </div>
            </div>

            <!-- Category list -->
            <ul class="space-y-2 max-h-64 overflow-y-auto custom-scroll">
              <li v-for="cat in categoryStats" :key="cat.name" class="flex items-center gap-3">
                <div class="flex-1">
                  <div class="flex items-center justify-between text-xs mb-1">
                    <span class="font-medium text-gray-100 truncate">
                      {{ cat.name }}
                    </span>
                    <span class="text-gray-400 whitespace-nowrap">
                      ${{ fmt(cat.value) }} • {{ cat.pct.toFixed(1) }}%
                    </span>
                  </div>
                  <div class="mt-0.5 h-1.5 rounded-full bg-[#26292E] overflow-hidden">
                    <div class="h-full rounded-full bg-indigo-500" :style="{ width: Math.max(cat.pct, 4) + '%' }"></div>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </section>

      <!-- 🧠 Advanced Insights Panel -->
      <section v-if="!loading && expenses.length && smartInsights.length" class="panel mt-10 animate-slide-up">
        <h3 class="panel-title">Advanced Insights</h3>
        <ul class="space-y-2 text-sm text-gray-300">
          <li v-for="(ins, idx) in smartInsights" :key="idx" class="flex items-start gap-2">
            <span class="text-indigo-400 mt-0.5">•</span>
            <span>{{ ins }}</span>
          </li>
        </ul>
      </section>
    </div>
  </div>
</template>

<script setup>
import {
  ref,
  onMounted,
  onUnmounted,
  computed,
  nextTick,
} from "vue";
import Chart from "chart.js/auto";
import api from "../api";

// ===== STATE =====
const rawExpenses = ref([]); // all user expenses
const expenses = ref([]); // filtered by [from,to]
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

// ===== PRESET HANDLING =====
function presetClass(label) {
  return [
    "px-4 py-2 rounded-lg border text-sm md:text-base transition",
    activePreset.value === label
      ? "bg-indigo-600 text-white border-indigo-500 shadow"
      : "bg-[#1D1F22] text-gray-300 border-[#35383E] hover:text-white hover:border-indigo-500",
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
    from.value = new Date(now.getFullYear(), 0, 1)
      .toISOString()
      .slice(0, 10);
    to.value = now.toISOString().slice(0, 10);
  } else if (typeof kind === "number") {
    const start = new Date(now);
    start.setDate(start.getDate() - kind);
    from.value = start.toISOString().slice(0, 10);
    to.value = now.toISOString().slice(0, 10);
  }

  refresh();
}

// ===== RANGE / PREVIOUS PERIOD HELPERS =====
const rangeInfo = computed(() => {
  const start = new Date(from.value + "T00:00:00");
  const end = new Date(to.value + "T23:59:59");
  const msPerDay = 1000 * 60 * 60 * 24;
  const lengthDays = Math.max(
    1,
    Math.round((end.getTime() - start.getTime()) / msPerDay) + 1
  );

  const prevEnd = new Date(start.getTime() - msPerDay);
  const prevStart = new Date(prevEnd.getTime() - msPerDay * (lengthDays - 1));

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

// ===== CORE METRICS =====
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

// 🔢 Category stats for right-side panel (B3/C1)
const categoryStats = computed(() => {
  if (!expenses.value.length) return [];
  const total = totalSpent.value || 0;
  const map = {};
  for (const e of expenses.value) {
    const cat = e.category || "Uncategorized";
    map[cat] = (map[cat] || 0) + Number(e.amount || 0);
  }
  return Object.entries(map)
    .map(([name, value]) => ({
      name,
      value,
      pct: total ? (value / total) * 100 : 0,
    }))
    .sort((a, b) => b.value - a.value);
});

const top3Share = computed(() => {
  if (!categoryStats.value.length || !totalSpent.value) return 0;
  const top3Total = categoryStats.value
    .slice(0, 3)
    .reduce((sum, c) => sum + c.value, 0);
  return (top3Total / totalSpent.value) * 100;
});

// ===== HYBRID FORECAST (LINEAR + MOVING AVG) =====
function buildDailySeries(list) {
  const map = {};
  for (const e of list) {
    const key = new Date(e.date).toISOString().slice(0, 10);
    map[key] = (map[key] || 0) + Number(e.amount || 0);
  }
  const dates = Object.keys(map).sort();
  const values = dates.map((d) => map[d]);
  return { dates, values };
}

function linearRegression(values) {
  const n = values.length;
  if (n < 2) {
    return { slope: 0, intercept: values[0] || 0 };
  }
  const xs = Array.from({ length: n }, (_, i) => i);
  const meanX = xs.reduce((a, b) => a + b, 0) / n;
  const meanY = values.reduce((a, b) => a + b, 0) / n;

  let num = 0;
  let den = 0;
  for (let i = 0; i < n; i++) {
    const dx = xs[i] - meanX;
    const dy = values[i] - meanY;
    num += dx * dy;
    den += dx * dx;
  }
  const slope = den === 0 ? 0 : num / den;
  const intercept = meanY - slope * meanX;
  return { slope, intercept };
}

// Use last 90 days of raw data for forecast
const forecastNext30 = computed(() => {
  if (!rawExpenses.value.length) return 0;

  const now = new Date();
  const start = new Date(now);
  start.setDate(start.getDate() - 89);
  const last90 = getRangeExpenses(start, now);
  const series = buildDailySeries(last90);

  if (!series.values.length) return 0;

  const { values } = series;
  const avg = values.reduce((a, b) => a + b, 0) / values.length;

  const { slope, intercept } = linearRegression(values);
  const n = values.length;

  // Linear forecast for next 30 days
  let linearTotal = 0;
  for (let k = 1; k <= 30; k++) {
    const y = slope * (n - 1 + k) + intercept;
    linearTotal += Math.max(0, y);
  }

  const avgTotal = avg * 30;

  // Hybrid: mostly linear, slightly smoothed by average
  const hybrid = 0.7 * linearTotal + 0.3 * avgTotal;
  return hybrid;
});

const forecastVsCurrentText = computed(() => {
  if (!totalSpent.value) return "no current period data";
  const diff = forecastNext30.value - totalSpent.value;
  const pct = (diff / totalSpent.value) * 100;
  const absPct = Math.abs(pct).toFixed(1);
  if (diff > 0) return `+${absPct}% vs this period`;
  if (diff < 0) return `-${absPct}% vs this period`;
  return "roughly flat vs this period";
});

// ===== SMART SEASONAL DETECTION =====
const seasonalInsights = computed(() => {
  const list = [];
  if (!rawExpenses.value.length) return list;

  // Weekend vs weekday
  let weekendTotal = 0;
  let weekendCount = 0;
  let weekdayTotal = 0;
  let weekdayCount = 0;

  for (const e of rawExpenses.value) {
    const d = new Date(e.date);
    const day = d.getDay(); // 0 = Sun, 6 = Sat
    const amt = Number(e.amount || 0);
    if (day === 0 || day === 6) {
      weekendTotal += amt;
      weekendCount++;
    } else {
      weekdayTotal += amt;
      weekdayCount++;
    }
  }

  const weekendAvg = weekendCount ? weekendTotal / weekendCount : 0;
  const weekdayAvg = weekdayCount ? weekdayTotal / weekdayCount : 0;

  if (weekendAvg && weekdayAvg) {
    const higherWeekend = weekendAvg > weekdayAvg;
    const diffPct =
      ((Math.max(weekendAvg, weekdayAvg) - Math.min(weekendAvg, weekdayAvg)) /
        Math.min(weekendAvg, weekdayAvg)) *
      100;

    if (diffPct >= 20) {
      list.push(
        `You tend to spend about ${diffPct.toFixed(
          1
        )}% more on ${higherWeekend ? "weekends" : "weekdays"} than on ${higherWeekend ? "weekdays" : "weekends"
        }.`
      );
    }
  }

  // Payday windows (1–3 and 15–17 of month)
  let paydayTotal = 0;
  let paydayCount = 0;
  let restTotal = 0;
  let restCount = 0;

  for (const e of rawExpenses.value) {
    const d = new Date(e.date);
    const day = d.getDate();
    const amt = Number(e.amount || 0);
    const inPayWindow =
      (day >= 1 && day <= 3) || (day >= 15 && day <= 17);
    if (inPayWindow) {
      paydayTotal += amt;
      paydayCount++;
    } else {
      restTotal += amt;
      restCount++;
    }
  }

  const paydayAvg = paydayCount ? paydayTotal / paydayCount : 0;
  const restAvg = restCount ? restTotal / restCount : 0;

  if (paydayAvg && restAvg && paydayAvg > restAvg * 1.3) {
    const bumpPct = ((paydayAvg - restAvg) / restAvg) * 100;
    list.push(
      `Spending spikes around common payday windows (1–3 & 15–17), about ${bumpPct.toFixed(
        1
      )}% higher than other days.`
    );
  }

  // Category seasonality by month
  const byCatMonth = {};
  for (const e of rawExpenses.value) {
    const d = new Date(e.date);
    const m = d.getMonth(); // 0-11
    const cat = e.category;
    const amt = Number(e.amount || 0);
    const key = `${cat}::${m}`;
    byCatMonth[key] = (byCatMonth[key] || 0) + amt;
  }

  const perCat = {};
  for (const key of Object.keys(byCatMonth)) {
    const [cat, mStr] = key.split("::");
    const m = Number(mStr);
    if (!perCat[cat]) perCat[cat] = {};
    perCat[cat][m] = byCatMonth[key];
  }

  for (const [cat, months] of Object.entries(perCat)) {
    const vals = Object.values(months);
    if (vals.length < 3) continue; // need some history
    const total = vals.reduce((a, b) => a + b, 0);
    const avg = total / vals.length;

    let bestMonth = null;
    let bestVal = 0;
    for (const [mStr, v] of Object.entries(months)) {
      const m = Number(mStr);
      if (!bestMonth || v > bestVal) {
        bestMonth = m;
        bestVal = v;
      }
    }

    if (bestVal >= avg * 1.5 && bestVal > 50) {
      const monthName = new Date(2024, bestMonth, 1).toLocaleString(
        undefined,
        { month: "long" }
      );
      list.push(
        `"${cat}" shows a seasonal bump: ${monthName} is about ${(
          (bestVal / avg - 1) *
          100
        ).toFixed(1)}% higher than its usual monthly level.`
      );
    }
  }

  return list;
});

// 🧠 Smart Insights = performance + seasonal
const smartInsights = computed(() => {
  const list = [];

  if (!expenses.value.length) return list;

  // Performance / comparison
  if (deltaPct.value !== null) {
    if (deltaAbs.value > 0) {
      list.push(
        `You spent ${deltaPctText.value.toLowerCase()} compared with the previous period (+$${fmt(
          deltaAbs.value
        )}).`
      );
    } else if (deltaAbs.value < 0) {
      list.push(
        `Nice! Spending is ${deltaPctText.value.toLowerCase()} vs the previous period (-$${fmt(
          Math.abs(deltaAbs.value)
        )}).`
      );
    }
  }

  if (topCategory.value) {
    list.push(
      `Your highest-spend category in this range is "${topCategory.value.name}" with $${fmt(
        topCategory.value.value
      )}.`
    );
  }

  if (biggestIncreaseCategory.value) {
    list.push(
      `"${biggestIncreaseCategory.value.name}" increased the most vs the previous period (+$${fmt(
        biggestIncreaseCategory.value.diff
      )}).`
    );
  }

  const { lengthDays } = rangeInfo.value;
  if (lengthDays >= 7) {
    list.push(
      `On average, you’re spending about $${fmt(
        avgPerDay.value
      )} per day in this selected range.`
    );
  }

  // Forecast
  if (forecastNext30.value > 0) {
    list.push(
      `If current patterns hold, you’re on track to spend roughly $${fmt(
        forecastNext30.value
      )} over the next 30 days (${forecastVsCurrentText.value}).`
    );
  }

  // Seasonal insights
  list.push(...seasonalInsights.value);

  return list;
});

// ===== CHARTS =====
const trendRef = ref(null);
const donutRef = ref(null);

let trendChart;
let donutChart;

function destroyCharts() {
  if (trendChart) {
    trendChart.destroy();
    trendChart = null;
  }
  if (donutChart) {
    donutChart.destroy();
    donutChart = null;
  }
}

async function drawCharts() {
  destroyCharts();
  if (!expenses.value.length) return;

  await nextTick();

  // Build daily series for current range
  const { start, end } = rangeInfo.value;
  const msPerDay = 1000 * 60 * 60 * 24;
  const days = [];
  const dayMap = {};

  let cursor = new Date(start);
  while (cursor <= end) {
    const key = cursor.toLocaleDateString();
    days.push(key);
    dayMap[key] = 0;
    cursor = new Date(cursor.getTime() + msPerDay);
  }

  for (const e of expenses.value) {
    const key = new Date(e.date).toLocaleDateString();
    if (dayMap[key] != null) {
      dayMap[key] += Number(e.amount || 0);
    }
  }

  const actualLabels = days;
  const actualValues = actualLabels.map((d) => dayMap[d] || 0);

  // Forecast daily series for next 30d using regression we already computed
  const now = new Date(end);
  const futureLabels = [];
  for (let i = 1; i <= 30; i++) {
    const d = new Date(now.getTime() + i * msPerDay);
    futureLabels.push(d.toLocaleDateString());
  }

  // Build linear regression using last 90 days raw data
  const start90 = new Date(end);
  start90.setDate(start90.getDate() - 89);
  const series90 = buildDailySeries(getRangeExpenses(start90, end));
  let forecastDaily = [];

  if (series90.values.length >= 2) {
    const { values } = series90;
    const { slope, intercept } = linearRegression(values);
    const n = values.length;

    for (let k = 1; k <= 30; k++) {
      const y = Math.max(0, slope * (n - 1 + k) + intercept);
      forecastDaily.push(y);
    }
  } else {
    const avg = actualValues.length
      ? actualValues.reduce((a, b) => a + b, 0) / actualValues.length
      : 0;
    forecastDaily = Array(30).fill(avg);
  }

  const trendLabels = [...actualLabels, ...futureLabels];

  const actualData = [
    ...actualValues,
    ...Array(futureLabels.length).fill(null),
  ];
  const forecastData = [
    ...Array(actualLabels.length).fill(null),
    ...forecastDaily,
  ];

  if (trendRef.value) {
    trendChart = new Chart(trendRef.value, {
      type: "line",
      data: {
        labels: trendLabels,
        datasets: [
          {
            label: "Actual",
            data: actualData,
            borderColor: "rgba(129,140,248,0.95)",
            backgroundColor: "rgba(129,140,248,0.15)",
            tension: 0.35,
            fill: true,
            pointRadius: 2,
          },
          {
            label: "Forecast (next 30d)",
            data: forecastData,
            borderColor: "rgba(236,72,153,0.9)",
            borderDash: [6, 4],
            pointRadius: 0,
            fill: false,
          },
        ],
      },
      options: {
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: { color: "#d1d5db" },
          },
        },
        scales: {
          x: {
            ticks: { color: "#9ca3af" },
            grid: { color: "rgba(255,255,255,0.06)" },
          },
          y: {
            ticks: { color: "#9ca3af" },
            grid: { color: "rgba(255,255,255,0.06)" },
          },
        },
      },
    });
  }

  // Donut: sum by category for current range
  const catMap = {};
  for (const e of expenses.value) {
    catMap[e.category] = (catMap[e.category] || 0) + Number(e.amount || 0);
  }

  const catLabels = Object.keys(catMap);
  const catVals = Object.values(catMap);

  const colors = [
    "#818CF8",
    "#EC4899",
    "#F59E0B",
    "#10B981",
    "#3B82F6",
    "#8B5CF6",
    "#F97316",
    "#22C55E",
  ];

  if (donutRef.value && catLabels.length) {
    donutChart = new Chart(donutRef.value, {
      type: "doughnut",
      data: {
        labels: catLabels,
        datasets: [
          {
            data: catVals,
            backgroundColor: colors,
          },
        ],
      },
      options: {
        plugins: {
          legend: { labels: { color: "#d1d5db" } },
        },
      },
    });
  }
}

// ===== EXPORTS =====
function exportChartPng(refEl) {
  if (!refEl.value) return;
  const url = refEl.value.toDataURL("image/png");
  const link = document.createElement("a");
  link.download = "chart.png";
  link.href = url;
  link.click();
}

// CSV of current-period expenses
function exportCsv() {
  if (!expenses.value.length) return;

  const rows = [
    ["Date", "Title", "Category", "Amount"],
    ...expenses.value.map((e) => [
      new Date(e.date).toISOString().slice(0, 10),
      e.title || "",
      e.category || "",
      Number(e.amount || 0).toFixed(2),
    ]),
  ];

  const csv = rows
    .map((row) =>
      row
        .map((field) =>
          `"${String(field).replace(/"/g, '""')}"`
        )
        .join(",")
    )
    .join("\n");

  const blob = new Blob([csv], {
    type: "text/csv;charset=utf-8;",
  });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = "expense-report.csv";
  link.click();
  URL.revokeObjectURL(link.href);
}

// PDF: open a new window, embed charts as images + summary + table, then let user "Save as PDF"
function exportPdf() {
  const win = window.open("", "_blank");
  if (!win) return;

  const trendImg = trendRef.value
    ? trendRef.value.toDataURL("image/png")
    : null;
  const donutImg = donutRef.value
    ? donutRef.value.toDataURL("image/png")
    : null;

  const safe = (s) =>
    String(s || "").replace(/&/g, "&amp;").replace(/</g, "&lt;");

  const rowsHtml = expenses.value
    .map(
      (e) => `
      <tr>
        <td>${new Date(e.date).toLocaleDateString()}</td>
        <td>${safe(e.title)}</td>
        <td>${safe(e.category)}</td>
        <td>$${fmt(e.amount)}</td>
      </tr>`
    )
    .join("");

  win.document.write(`
    <html>
      <head>
        <title>Expense Report</title>
        <style>
          body {
            font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
            padding: 24px;
            color: #111827;
          }
          h1 { font-size: 24px; margin-bottom: 4px; }
          h2 { font-size: 18px; margin-top: 24px; margin-bottom: 8px; }
          .muted { color: #6b7280; font-size: 12px; margin-bottom: 16px; }
          .summary { font-size: 14px; margin-bottom: 8px; }
          table {
            border-collapse: collapse;
            width: 100%;
            margin-top: 12px;
            font-size: 12px;
          }
          th, td {
            border: 1px solid #d1d5db;
            padding: 6px 8px;
            text-align: left;
          }
          th { background: #f3f4f6; }
          img { max-width: 100%; margin-top: 8px; }
          ul { padding-left: 16px; font-size: 12px; }
        </style>
      </head>
      <body>
        <h1>Expense Report</h1>
        <p class="muted">Range: ${rangeLabel.value}</p>
        <p class="summary">
          Total Spent: $${fmt(totalSpent.value)} |
          Top Category: ${safe(topCategory.value?.name || "—")} |
          Daily Avg: $${fmt(avgPerDay.value)} |
          30-Day Forecast: $${fmt(forecastNext30.value)}
        </p>

        ${trendImg
      ? `<h2>Spending Trend & 30-Day Forecast</h2><img src="${trendImg}" />`
      : ""
    }
        ${donutImg
      ? `<h2>Category Breakdown</h2><img src="${donutImg}" />`
      : ""
    }

        ${smartInsights.value.length
      ? `<h2>Key Insights</h2>
               <ul>${smartInsights.value
        .map((x) => `<li>${safe(x)}</li>`)
        .join("")}</ul>`
      : ""
    }

        <h2>Transactions</h2>
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Title</th>
              <th>Category</th>
              <th>Amount</th>
            </tr>
          </thead>
          <tbody>
            ${rowsHtml}
          </tbody>
        </table>
      </body>
    </html>
  `);

  win.document.close();
  win.focus();
  win.print(); // user can "Save as PDF" here
}

// ===== FETCH & REFRESH =====
async function refresh() {
  loading.value = true;
  try {
    const { data } = await api.get("/api/expense");
    rawExpenses.value = data || [];

    const f = new Date(from.value + "T00:00:00");
    const t = new Date(to.value + "T23:59:59");

    expenses.value = rawExpenses.value.filter((e) => {
      const d = new Date(e.date);
      return d >= f && d <= t;
    });

    await drawCharts();
  } finally {
    loading.value = false;
  }
}

// ===== LIFECYCLE =====
onMounted(refresh);
onUnmounted(() => {
  destroyCharts();
});

// ===== UTILS =====
const fmt = (n) => Number(n || 0).toFixed(2);
</script>

<style scoped>
/* ---------------------------------------------------
   🎨 GLOBAL ANIMATIONS
--------------------------------------------------- */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(12px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 0.6s ease-out forwards;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slide-up {
  animation: slideUp 0.7s ease-out forwards;
}

/* ---------------------------------------------------
   🧰 INPUTS & BUTTONS
--------------------------------------------------- */
.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.btn-graphite {
  @apply px-5 py-2.5 rounded-lg font-semibold bg-indigo-600 text-white shadow-md hover:bg-indigo-500 hover:shadow-indigo-500/30 transition-all duration-200;
}

.export-btn {
  @apply mt-4 text-xs text-gray-400 underline hover:text-indigo-400 transition;
}

/* ---------------------------------------------------
   🧱 CARDS & PANELS
--------------------------------------------------- */
.card-graphite {
  @apply p-6 rounded-2xl shadow-lg backdrop-blur-xl bg-[#1D1F22]/80 border border-[#35383E] transition hover:shadow-indigo-500/20;
}

.panel {
  @apply p-6 rounded-2xl shadow-lg backdrop-blur-xl bg-[#1D1F22]/80 border border-[#35383E];
}

.panel-title {
  @apply text-white font-semibold text-lg mb-4;
}

/* ---------------------------------------------------
   📊 CHART CONTAINERS
--------------------------------------------------- */
.chart-wrap {
  @apply relative w-full h-[260px] md:h-[320px];
}

.chart-wrap--tall {
  @apply h-[320px] md:h-[380px];
}

/* ---------------------------------------------------
   ⏳ LOADING SKELETONS
--------------------------------------------------- */
.skeleton-card {
  @apply h-24 rounded-2xl bg-gray-700/20 border border-gray-700 animate-pulse;
}

/* ---------------------------------------------------
   📜 SCROLLABLE LISTS
--------------------------------------------------- */
.custom-scroll {
  scrollbar-width: thin;
  scrollbar-color: #4b5563 transparent;
}

.custom-scroll::-webkit-scrollbar {
  width: 6px;
}

.custom-scroll::-webkit-scrollbar-track {
  background: transparent;
}

.custom-scroll::-webkit-scrollbar-thumb {
  background-color: #4b5563;
  border-radius: 9999px;
}

/* Center ONLY the donut chart */
.donut-center {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
