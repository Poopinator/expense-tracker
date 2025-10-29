<template>
  <div class="relative min-h-screen bg-[#141517] text-gray-100 font-sans overflow-hidden">
    <!-- 🌌 Custom Canvas Particle Background -->
    <ParticleBackground />

    <!-- Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <!-- 💸 Page Content -->
    <div class="relative z-10 p-8 flex flex-col items-center">
      <!-- Header -->
      <header class="w-full max-w-6xl mb-10 text-center">
        <h1 class="text-4xl font-bold text-white tracking-tight mb-2">💸 Expense Dashboard</h1>
        <p class="text-gray-400 text-sm">Track your spending beautifully</p>
      </header>

      <!-- Add Expense -->
      <section class="w-full max-w-4xl bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-8 mb-8">
        <h2 class="text-2xl font-semibold mb-6 text-center text-white">Add Expense</h2>

        <form @submit.prevent="addExpense" class="grid grid-cols-1 md:grid-cols-2 gap-4 items-end">
          <div class="flex flex-col">
            <label class="text-sm mb-1 text-gray-400">Title</label>
            <input v-model="title" placeholder="e.g. Dinner at ToriYard" class="input-graphite" />
          </div>

          <div class="flex flex-col">
            <label class="text-sm mb-1 text-gray-400">Category</label>
            <input v-model="category" placeholder="Food, Transport..." class="input-graphite" />
          </div>

          <div class="flex flex-col">
            <label class="text-sm mb-1 text-gray-400">Amount ($)</label>
            <input v-model.number="amount" type="number" placeholder="0.00" class="input-graphite" />
          </div>

          <div class="flex flex-col">
            <label class="text-sm mb-1 text-gray-400">Date</label>
            <input v-model="date" type="date" class="input-graphite" />
          </div>

          <div class="md:col-span-2 flex justify-end">
            <button type="submit" class="btn-graphite">➕ Add Expense</button>
          </div>
        </form>
      </section>

      <!-- Expense List -->
      <section class="w-full max-w-5xl bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-8 mb-8">
        <div class="flex flex-col md:flex-row justify-between items-center mb-6">
          <h2 class="text-2xl font-semibold text-white">Your Expenses</h2>

          <select v-model="filter" class="select-graphite mt-4 md:mt-0">
            <option value="daily">Daily</option>
            <option value="monthly">Monthly</option>
            <option value="all">All</option>
          </select>
        </div>

        <ul v-if="filteredExpenses.length" class="space-y-3">
          <li
            v-for="e in filteredExpenses"
            :key="e.id"
            class="flex justify-between items-center bg-[#26292E]/80 text-gray-200 rounded-lg px-5 py-4 border border-[#35383E] hover:border-indigo-500 transition-all duration-150"
          >
            <div>
              <h3 class="font-semibold text-gray-100">{{ e.title }}</h3>
              <p class="text-sm text-gray-500">
                {{ e.category }} • {{ new Date(e.date).toLocaleDateString() }}
              </p>
            </div>
            <div class="flex items-center gap-4">
              <span class="font-semibold text-indigo-400 text-lg">${{ e.amount.toFixed(2) }}</span>
              <button
                @click="deleteExpense(e.id)"
                class="text-gray-500 hover:text-red-500 transition"
                title="Delete"
              >
                🗑️
              </button>
            </div>
          </li>
        </ul>

        <p v-else class="text-center text-gray-500 mt-6">No expenses found for this filter.</p>
      </section>

      <!-- Summary Cards -->
      <section class="grid md:grid-cols-3 gap-6 w-full max-w-5xl mb-10">
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">🕒 Today</h3>
          <p class="text-3xl font-bold text-indigo-400">${{ summary.today.toFixed(2) }}</p>
        </div>
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">📅 This Month</h3>
          <p class="text-3xl font-bold text-indigo-400">${{ summary.month.toFixed(2) }}</p>
        </div>
        <div class="card-graphite">
          <h3 class="text-gray-400 text-sm">🌍 Total</h3>
          <p class="text-3xl font-bold text-indigo-400">${{ summary.total.toFixed(2) }}</p>
        </div>
      </section>

      <!-- Chart -->
      <section class="w-full max-w-5xl bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-8 text-center">
        <h2 class="text-2xl font-semibold mb-4 text-white">Spending by Category</h2>
        <canvas id="expenseChart" class="max-w-[500px] mx-auto"></canvas>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue"
import api from "../api"
import Chart from "chart.js/auto"
import ParticleBackground from "../components/ParticleBackground.vue"

const expenses = ref([])
const title = ref("")
const category = ref("")
const amount = ref("")
const date = ref("")
const filter = ref("daily")

const summary = ref({ today: 0, month: 0, total: 0 })
let chartInstance = null

const fetchExpenses = async () => {
  const res = await api.get("/api/expense")
  expenses.value = res.data
  updateSummary()
  updateChart()
}

const addExpense = async () => {
  if (!title.value || !amount.value || !category.value || !date.value) return
  await api.post("/api/expense", {
    title: title.value,
    category: category.value,
    amount: amount.value,
    date: date.value,
  })
  title.value = category.value = amount.value = date.value = ""
  await fetchExpenses()
}

const deleteExpense = async (id) => {
  await api.delete(`/api/expense/${id}`)
  await fetchExpenses()
}

const updateSummary = () => {
  const now = new Date()
  let todaySum = 0,
    monthSum = 0,
    totalSum = 0
  expenses.value.forEach((e) => {
    const d = new Date(e.date)
    totalSum += e.amount
    if (d.toDateString() === now.toDateString()) todaySum += e.amount
    if (d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear())
      monthSum += e.amount
  })
  summary.value = { today: todaySum, month: monthSum, total: totalSum }
}

const updateChart = () => {
  const ctx = document.getElementById("expenseChart")
  if (!ctx) return
  const categories = {}
  expenses.value.forEach((e) => {
    categories[e.category] = (categories[e.category] || 0) + e.amount
  })

  const data = {
    labels: Object.keys(categories),
    datasets: [
      {
        label: "Spending by Category",
        data: Object.values(categories),
        backgroundColor: [
          "#6366F1",
          "#8B5CF6",
          "#EC4899",
          "#F59E0B",
          "#10B981",
          "#3B82F6",
        ],
        borderWidth: 1,
      },
    ],
  }

  if (chartInstance) chartInstance.destroy()
  chartInstance = new Chart(ctx, {
    type: "doughnut",
    data,
    options: {
      plugins: {
        legend: { labels: { color: "#fff" } },
      },
    },
  })
}

const filteredExpenses = computed(() => {
  const now = new Date()
  if (filter.value === "daily") {
    return expenses.value.filter(
      (e) => new Date(e.date).toDateString() === now.toDateString()
    )
  }
  if (filter.value === "monthly") {
    return expenses.value.filter((e) => {
      const d = new Date(e.date)
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear()
    })
  }
  return expenses.value
})

onMounted(fetchExpenses)
watch(expenses, updateSummary)
</script>

<style scoped>
.input-graphite {
  @apply px-4 py-3 rounded-xl bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}
.select-graphite {
  @apply px-4 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}
.btn-graphite {
  @apply px-8 py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow-md hover:shadow-indigo-500/30 transition-all duration-300;
}
.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 text-center shadow-md hover:shadow-indigo-500/20 transition-all;
}
</style>
