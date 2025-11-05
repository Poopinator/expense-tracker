<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 overflow-hidden">
    <div class="particle-bg"></div>
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <div class="relative z-10 mx-auto max-w-7xl px-5 py-8">
      <header class="mb-6">
        <h1 class="text-3xl font-bold text-white">📊 Reports</h1>
        <p class="text-gray-400 mt-1">Trends and breakdowns across your selected period.</p>
      </header>

      <!-- filters -->
      <div class="flex flex-wrap gap-3 mb-6">
        <button
          v-for="p in presets"
          :key="p.label"
          class="px-3 py-2 rounded-md bg-[#1D1F22] border border-[#35383E] text-gray-300 hover:text-white"
          @click="applyPreset(p.days)"
        >
          {{ p.label }}
        </button>
        <input type="date" v-model="from" class="select-graphite" />
        <input type="date" v-model="to" class="select-graphite" />
        <button class="btn-graphite" @click="refresh">Apply</button>
      </div>

      <!-- insights -->
      <div class="grid md:grid-cols-3 gap-4 mb-6">
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">Total Spent</h3>
          <p class="text-2xl font-bold text-indigo-400">${{ fmt(totalSpent) }}</p>
        </div>
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">Top Category</h3>
          <p class="text-2xl font-bold text-indigo-400">{{ topCategory?.name || '—' }}</p>
        </div>
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">Most Active Day</h3>
          <p class="text-2xl font-bold text-indigo-400">{{ busiestDay || '—' }}</p>
        </div>
      </div>

      <!-- charts -->
      <section class="grid md:grid-cols-2 gap-6">
        <div class="panel">
          <h3 class="panel-title">Spending Trend</h3>
          <div class="chart-wrap">
            <canvas ref="trendRef"></canvas>
          </div>
        </div>

        <div class="panel">
          <h3 class="panel-title">By Category</h3>
          <div class="chart-wrap">
            <canvas ref="barRef"></canvas>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import Chart from 'chart.js/auto'
import api from '../api'

const expenses = ref([])
const loading = ref(false)

const today = new Date()
const to = ref(new Date().toISOString().slice(0,10))
const from = ref(new Date(today.getFullYear(), today.getMonth(), 1).toISOString().slice(0,10)) // default: this month

const presets = [
  { label: 'This Month', days: 'month' },
  { label: 'Last 30d',   days: 30 },
  { label: 'Last 90d',   days: 90 },
  { label: 'YTD',        days: 'ytd' },
]

function applyPreset(kind) {
  const now = new Date()
  if (kind === 'month') {
    from.value = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0,10)
    to.value   = now.toISOString().slice(0,10)
  } else if (kind === 'ytd') {
    from.value = new Date(now.getFullYear(), 0, 1).toISOString().slice(0,10)
    to.value   = now.toISOString().slice(0,10)
  } else if (typeof kind === 'number') {
    const start = new Date(now)
    start.setDate(start.getDate() - kind)
    from.value = start.toISOString().slice(0,10)
    to.value   = now.toISOString().slice(0,10)
  }
  refresh()
}

async function refresh() {
  loading.value = true
  try {
    // we’ll just fetch all and filter client-side (simple and robust)
    const { data } = await api.get('/api/expense')
    const f = new Date(from.value + 'T00:00:00')
    const t = new Date(to.value   + 'T23:59:59')
    expenses.value = (data || []).filter(e => {
      const d = new Date(e.date)
      return d >= f && d <= t
    })
    drawCharts()
  } finally {
    loading.value = false
  }
}

const totalSpent = computed(() =>
  expenses.value.reduce((s, e) => s + Number(e.amount || 0), 0)
)

const topCategory = computed(() => {
  const byCat = {}
  for (const e of expenses.value) {
    byCat[e.category] = (byCat[e.category] || 0) + Number(e.amount || 0)
  }
  let best = null
  for (const [name, val] of Object.entries(byCat)) {
    if (!best || val > best.value) best = { name, value: val }
  }
  return best
})

const busiestDay = computed(() => {
  const byDay = {}
  for (const e of expenses.value) {
    const k = new Date(e.date).toLocaleDateString()
    byDay[k] = (byDay[k] || 0) + Number(e.amount || 0)
  }
  let best = null
  for (const [k, v] of Object.entries(byDay)) {
    if (!best || v > best.value) best = { day: k, value: v }
  }
  return best?.day
})

// charts
const trendRef = ref(null)
const barRef   = ref(null)
let trendChart, barChart

function drawCharts() {
  // trend: sum by date
  const dayMap = {}
  const sorted = [...expenses.value].sort((a,b) => new Date(a.date) - new Date(b.date))
  for (const e of sorted) {
    const key = new Date(e.date).toLocaleDateString()
    dayMap[key] = (dayMap[key] || 0) + Number(e.amount || 0)
  }
  const dayLabels = Object.keys(dayMap)
  const dayVals   = Object.values(dayMap)

  if (trendChart) trendChart.destroy()
  if (trendRef.value) {
    trendChart = new Chart(trendRef.value, {
      type: 'line',
      data: {
        labels: dayLabels,
        datasets: [{
          label: 'Spending',
          data: dayVals,
          borderColor: '#6366F1',
          backgroundColor: 'rgba(99,102,241,0.2)',
          tension: 0.35,
          fill: true,
          pointRadius: 2
        }]
      },
      options: {
        maintainAspectRatio: false,
        scales: {
          x: { ticks: { color: '#9ca3af' }, grid: { color: 'rgba(255,255,255,0.06)' } },
          y: { ticks: { color: '#9ca3af' }, grid: { color: 'rgba(255,255,255,0.06)' } }
        },
        plugins: { legend: { labels: { color: '#d1d5db' } } }
      }
    })
  }

  // bar: sum by category
  const catMap = {}
  for (const e of expenses.value) {
    catMap[e.category] = (catMap[e.category] || 0) + Number(e.amount || 0)
  }
  const catLabels = Object.keys(catMap)
  const catVals   = Object.values(catMap)

  if (barChart) barChart.destroy()
  if (barRef.value) {
    barChart = new Chart(barRef.value, {
      type: 'bar',
      data: {
        labels: catLabels,
        datasets: [{
          label: 'Amount',
          data: catVals,
          backgroundColor: ['#6366F1','#8B5CF6','#EC4899','#F59E0B','#10B981','#3B82F6'],
          borderWidth: 0
        }]
      },
      options: {
        maintainAspectRatio: false,
        plugins: { legend: { labels: { color: '#d1d5db' } } },
        scales: {
          x: { ticks: { color: '#9ca3af' }, grid: { color: 'rgba(255,255,255,0.06)' } },
          y: { ticks: { color: '#9ca3af' }, grid: { color: 'rgba(255,255,255,0.06)' } }
        }
      }
    })
  }
}

const fmt = (n) => Number(n || 0).toFixed(2)

onMounted(refresh)
watch([from, to], () => {}) // reactive if you want auto updates
</script>

<style scoped>
.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}
.btn-graphite {
  @apply px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow-md hover:shadow-indigo-500/30 transition;
}
.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 text-center shadow-md hover:shadow-indigo-500/20 transition;
}
.panel { @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-lg shadow-md p-6; }
.panel-title { @apply text-white font-semibold mb-3; }
.chart-wrap { @apply relative w-full h-[300px] md:h-[360px]; }
</style>
