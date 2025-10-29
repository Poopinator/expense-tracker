<template>
  <div class="min-h-screen bg-gradient-to-br from-indigo-600 via-purple-600 to-pink-400 p-6 flex flex-col items-center text-white">
    <!-- Header -->
    <div class="w-full max-w-5xl text-center mb-8">
      <h1 class="text-4xl font-extrabold mb-2 drop-shadow-md">💸 Expense Dashboard</h1>
      <p class="text-indigo-100/90">Add and view your daily, monthly, or all-time expenses easily</p>
    </div>

    <!-- Add Expense Section (TOP) -->
    <div
      class="backdrop-blur-md bg-white/20 border border-white/40 shadow-2xl rounded-2xl p-8 w-full max-w-4xl mb-10 transition hover:shadow-indigo-400/50"
    >
      <h2 class="text-2xl font-semibold mb-6 text-center text-white drop-shadow-md">Add New Expense</h2>

      <form @submit.prevent="addExpense" class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <input
          v-model="title"
          placeholder="Title"
          class="px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
        />
        <input
          v-model="category"
          placeholder="Category"
          class="px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
        />
        <input
          v-model.number="amount"
          type="number"
          placeholder="Amount ($)"
          class="px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
        />
        <input
          v-model="date"
          type="date"
          class="px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
        />

        <div class="md:col-span-2 flex justify-center">
          <button
            type="submit"
            class="w-full md:w-auto px-8 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold rounded-lg shadow-md hover:shadow-lg transition-all duration-300"
          >
            ➕ Add Expense
          </button>
        </div>
      </form>
    </div>

    <!-- Expense List Section -->
    <div
      class="backdrop-blur-md bg-white/20 border border-white/40 shadow-xl rounded-2xl p-8 w-full max-w-5xl mb-10 transition hover:shadow-indigo-400/50"
    >
      <div class="flex flex-col md:flex-row md:items-center justify-between mb-6">
        <h2 class="text-2xl font-semibold drop-shadow-md">Your Expenses</h2>

        <select
          v-model="filter"
          class="mt-4 md:mt-0 px-4 py-2 rounded-lg bg-white/80 text-gray-800 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
        >
          <option value="daily">Daily</option>
          <option value="monthly">Monthly</option>
          <option value="all">All</option>
        </select>
      </div>

      <ul v-if="filteredExpenses.length" class="space-y-3">
        <li
          v-for="e in filteredExpenses"
          :key="e.id"
          class="flex justify-between items-center bg-white/90 text-gray-800 rounded-lg px-5 py-3 shadow-sm hover:shadow-md transition"
        >
          <div>
            <h3 class="font-semibold text-gray-900">{{ e.title }}</h3>
            <p class="text-sm text-gray-600">
              {{ e.category }} • {{ new Date(e.date).toLocaleDateString() }}
            </p>
          </div>
          <div class="flex items-center gap-4">
            <span class="font-bold text-indigo-700">${{ e.amount.toFixed(2) }}</span>
            <button
              @click="deleteExpense(e.id)"
              class="text-red-500 hover:text-red-700 transition"
              title="Delete"
            >
              🗑️
            </button>
          </div>
        </li>
      </ul>

      <p v-else class="text-center text-indigo-100 mt-6">
        No expenses found for this filter.
      </p>
    </div>

    <!-- Summary Bar + Chart Section (BOTTOM) -->
    <div
      class="backdrop-blur-md bg-white/20 border border-white/40 shadow-xl rounded-2xl p-6 w-full max-w-5xl mb-10 flex flex-col sm:flex-row justify-around items-center text-center gap-6"
    >
      <div>
        <h3 class="text-lg font-semibold">🕒 Today</h3>
        <p class="text-2xl font-bold text-yellow-200">${{ summary.today.toFixed(2) }}</p>
      </div>
      <div>
        <h3 class="text-lg font-semibold">📅 This Month</h3>
        <p class="text-2xl font-bold text-yellow-200">${{ summary.month.toFixed(2) }}</p>
      </div>
      <div>
        <h3 class="text-lg font-semibold">🌍 Total</h3>
        <p class="text-2xl font-bold text-yellow-200">${{ summary.total.toFixed(2) }}</p>
      </div>
    </div>

    <div
      class="backdrop-blur-md bg-white/20 border border-white/40 shadow-xl rounded-2xl p-6 w-full max-w-5xl mb-20 flex flex-col items-center"
    >
      <h2 class="text-2xl font-semibold mb-4">Spending by Category</h2>
      <canvas id="expenseChart" class="max-w-[500px]"></canvas>
    </div>
  </div>
</template>


<script setup>
import { ref, computed, onMounted, watch } from "vue"
import api from "../api"
import Chart from "chart.js/auto"

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

// 💰 Summary Calculations
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

// 📊 Category Chart
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
        legend: {
          labels: {
            color: "#fff",
          },
        },
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
