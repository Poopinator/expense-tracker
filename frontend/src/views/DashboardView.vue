<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 font-sans overflow-hidden">
    <!-- 🌌 Tailwind Particle Background -->
    <div class="particle-bg"></div>

    <!-- 🌈 Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <!-- 💸 Dashboard Content -->
    <div class="relative z-10 px-5 py-8 mx-auto max-w-7xl">
      <!-- Header -->
      <header class="mb-8 text-center">
        <h1 class="text-3xl md:text-4xl font-bold tracking-tight text-white">💸 Expense Dashboard</h1>
        <p class="text-gray-400 mt-2">Manage expenses and track your budgets efficiently.</p>
      </header>

      <!-- Tabs -->
      <div class="flex justify-center mb-8 space-x-3">
        <button
          @click="activeTab = 'expenses'"
          :class="[
            'px-5 py-2 rounded-lg font-semibold transition',
            activeTab === 'expenses'
              ? 'bg-indigo-600 text-white shadow-md'
              : 'bg-[#1D1F22] text-gray-400 hover:text-white'
          ]"
        >
          Expenses
        </button>
        <button
          @click="activeTab = 'budgets'"
          :class="[
            'px-5 py-2 rounded-lg font-semibold transition',
            activeTab === 'budgets'
              ? 'bg-indigo-600 text-white shadow-md'
              : 'bg-[#1D1F22] text-gray-400 hover:text-white'
          ]"
        >
          Budgets
        </button>
      </div>

      <!-- ============================ -->
      <!-- 💰 EXPENSES TAB -->
      <!-- ============================ -->
      <section v-if="activeTab === 'expenses'">
        <!-- Add Expense -->
        <section
          class="bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-6 md:p-8 mb-8"
        >
          <h2 class="text-xl font-semibold text-white mb-5">Add New Expense</h2>
          <form @submit.prevent="onAdd" class="grid grid-cols-1 md:grid-cols-5 gap-3 md:gap-4">
            <input
              v-model.trim="title"
              class="input-graphite md:col-span-2"
              placeholder="Title (e.g. Lunch)"
              @keyup.enter="onAdd"
            />

            <!-- Category Dropdown -->
            <select v-model="category" class="select-graphite">
              <option disabled value="">Select Category</option>
              <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
            </select>

            <!-- Custom Category Field -->
            <input
              v-if="category === 'Other'"
              v-model.trim="customCategory"
              class="input-graphite"
              placeholder="Enter custom category"
            />

            <input
              v-model.number="amount"
              type="number"
              step="0.01"
              min="0"
              class="input-graphite"
              placeholder="Amount ($)"
              @keyup.enter="onAdd"
            />
            <input v-model="date" type="date" class="input-graphite" @keyup.enter="onAdd" />
            <div class="md:col-span-5 flex justify-end pt-2">
              <button type="submit" class="btn-graphite">➕ Add Expense</button>
            </div>
          </form>
          <p v-if="formError" class="text-rose-300 mt-3 text-sm">{{ formError }}</p>
        </section>

        <!-- Expense List -->
        <section
          class="bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-6 md:p-8 mb-8"
        >
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-4">
            <h2 class="text-xl font-semibold text-white">Your Expenses</h2>
            <div class="flex gap-3">
              <select v-model="filter" class="select-graphite">
                <option value="daily">Daily</option>
                <option value="monthly">Monthly</option>
                <option value="all">All</option>
              </select>
              <input v-model.trim="search" class="select-graphite" placeholder="Search title/category…" />
            </div>
          </div>

          <div v-if="loading" class="text-center text-gray-400 py-6">Loading expenses…</div>

          <ul v-else-if="filteredExpenses.length" class="space-y-3">
            <li
              v-for="e in filteredExpenses"
              :key="e.id"
              class="flex justify-between items-center bg-[#26292E]/80 rounded-lg px-4 py-3 border border-[#35383E] hover:border-indigo-500 transition"
            >
              <div class="min-w-0">
                <h3 class="font-semibold text-gray-100 truncate">{{ e.title }}</h3>
                <p class="text-xs text-gray-500 mt-0.5">{{ e.category }} • {{ fmtDate(e.date) }}</p>
              </div>
              <div class="flex items-center gap-4 shrink-0">
                <span class="font-semibold text-indigo-400 text-lg">${{ fmtAmt(e.amount) }}</span>
                <button
                  @click="onDelete(e.id)"
                  class="text-gray-500 hover:text-red-500 transition"
                  title="Delete"
                >
                  🗑️
                </button>
              </div>
            </li>
          </ul>

          <p v-else class="text-center text-gray-500 py-6">No expenses found for this view.</p>
        </section>

        <!-- Summary Cards -->
        <section class="grid md:grid-cols-3 gap-5 mb-8">
          <div class="card-graphite">
            <h3 class="text-gray-400 text-sm">🕒 Today</h3>
            <p class="text-3xl font-bold text-indigo-400">${{ fmtAmt(summary.today) }}</p>
          </div>
          <div class="card-graphite">
            <h3 class="text-gray-400 text-sm">📅 This Month</h3>
            <p class="text-3xl font-bold text-indigo-400">${{ fmtAmt(summary.month) }}</p>
          </div>
          <div class="card-graphite">
            <h3 class="text-gray-400 text-sm">🌍 Total</h3>
            <p class="text-3xl font-bold text-indigo-400">${{ fmtAmt(summary.total) }}</p>
          </div>
        </section>

        <!-- Charts -->
        <section class="grid md:grid-cols-2 gap-6">
          <div class="panel">
            <h3 class="panel-title">Spending by Category</h3>
            <div class="chart-wrap">
              <canvas ref="donutRef"></canvas>
            </div>
          </div>

          <div class="panel">
            <h3 class="panel-title">Spending Trend</h3>
            <div class="chart-wrap">
              <canvas ref="lineRef"></canvas>
            </div>
          </div>
        </section>
      </section>

      <!-- ============================ -->
      <!-- 📊 BUDGETS TAB -->
      <!-- ============================ -->
      <section
        v-else
        class="bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-6 md:p-8"
      >
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-6">
          <h2 class="text-xl font-semibold text-white">Your Budgets</h2>

          <form @submit.prevent="onSetBudget" class="flex gap-3">
            <select v-model="budgetCategory" class="select-graphite">
              <option disabled value="">Select Category</option>
              <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
            </select>
            <input v-model.number="budgetLimit" type="number" min="0" placeholder="Limit ($)" class="input-graphite" />
            <button type="submit" class="btn-graphite">💾 Save</button>
          </form>
        </div>

        <div v-if="budgetsLoading" class="text-center text-gray-400 py-6">Loading budgets…</div>

        <div v-else-if="budgetComparison.length" class="grid md:grid-cols-2 gap-4">
          <div
            v-for="b in budgetComparison"
            :key="b.category"
            class="p-4 rounded-xl border border-[#35383E] bg-[#26292E]/70 shadow-md hover:shadow-indigo-500/20 transition"
          >
            <div class="flex justify-between mb-2">
              <h3 class="font-semibold text-gray-100">{{ b.category }}</h3>
              <span :class="b.percentage > 100 ? 'text-red-400' : 'text-green-400'">
                {{ b.percentage.toFixed(1) }}%
              </span>
            </div>

            <BudgetBar :percentage="b.percentage" class="mb-3" />

            <p class="text-gray-400 text-sm">
              Spent: <span class="text-indigo-400">${{ fmtAmt(b.spent) }}</span> /
              Limit: <span class="text-gray-200">${{ fmtAmt(b.limit) }}</span><br />
              Remaining:
              <span :class="b.remaining < 0 ? 'text-red-400' : 'text-green-400'">
                ${{ fmtAmt(b.remaining) }}
              </span>
            </p>
          </div>
        </div>

        <p v-else class="text-center text-gray-500 py-6">No budgets set yet. Add one above 👆</p>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useToast } from "vue-toastification";
import Chart from "chart.js/auto";
import api from "../api";
import { getBudgets, compareBudgets, upsertBudget } from "../api/budget";
import BudgetBar from "../components/BudgetBar.vue";

const toast = useToast();

// Tabs
const activeTab = ref("expenses");

// Predefined categories
const categories = ref([
  "Food",
  "Transport",
  "Entertainment",
  "Shopping",
  "Bills",
  "Health",
  "Education",
  "Travel",
  "Utilities",
  "Other"
]);

// Expense state
const title = ref("");
const category = ref("");
const customCategory = ref("");
const amount = ref(null);
const date = ref("");
const formError = ref("");
const loading = ref(false);

// Filters
const filter = ref("daily");
const search = ref("");
const expenses = ref([]);

// Chart refs
const donutRef = ref(null);
const lineRef = ref(null);
let donutChart, lineChart;

// Fetch expenses
const fetchExpenses = async () => {
  loading.value = true;
  try {
    const { data } = await api.get("/api/expense");
    expenses.value = data || [];
  } finally {
    loading.value = false;
    updateCharts();
  }
};

// Add expense
const onAdd = async () => {
  formError.value = "";
  if (!title.value || !category.value || !amount.value || !date.value) {
    formError.value = "Please fill in all fields.";
    toast.warning("Missing fields");
    return;
  }
  if (category.value === "Other" && !customCategory.value.trim()) {
    toast.warning("Enter a custom category name");
    return;
  }
  try {
    await api.post("/api/expense", {
      title: title.value.trim(),
      category:
        category.value === "Other" ? customCategory.value.trim() : category.value.trim(),
      amount: Number(amount.value),
      date: date.value,
    });
    toast.success("Expense added 💰");
    title.value = category.value = customCategory.value = "";
    amount.value = null;
    date.value = "";
    await fetchExpenses();
  } catch {
    toast.error("Failed to add expense");
  }
};

// Delete expense
const onDelete = async (id) => {
  try {
    await api.delete(`/api/expense/${id}`);
    toast("Expense deleted 🗑️", { type: "info" });
    await fetchExpenses();
  } catch {
    toast.error("Failed to delete expense");
  }
};

// Filter logic
const filteredExpenses = computed(() => {
  const q = search.value.toLowerCase();
  const now = new Date();
  return expenses.value.filter((e) => {
    const d = new Date(e.date);
    const matchFilter =
      filter.value === "all" ||
      (filter.value === "daily" && d.toDateString() === now.toDateString()) ||
      (filter.value === "monthly" &&
        d.getMonth() === now.getMonth() &&
        d.getFullYear() === now.getFullYear());
    const matchText =
      !q || e.title.toLowerCase().includes(q) || e.category.toLowerCase().includes(q);
    return matchFilter && matchText;
  });
});

// Summary
const summary = computed(() => {
  const now = new Date();
  return expenses.value.reduce(
    (acc, e) => {
      const d = new Date(e.date);
      acc.total += e.amount;
      if (d.toDateString() === now.toDateString()) acc.today += e.amount;
      if (d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear())
        acc.month += e.amount;
      return acc;
    },
    { today: 0, month: 0, total: 0 }
  );
});

// Charts
const updateCharts = () => {
  const catMap = expenses.value.reduce(
    (m, e) => ((m[e.category] = (m[e.category] || 0) + e.amount), m),
    {}
  );
  const catLabels = Object.keys(catMap);
  const catVals = Object.values(catMap);

  const dayMap = expenses.value
    .slice()
    .sort((a, b) => new Date(a.date) - new Date(b.date))
    .reduce((m, e) => {
      const k = new Date(e.date).toLocaleDateString();
      m[k] = (m[k] || 0) + e.amount;
      return m;
    }, {});
  const dayLabels = Object.keys(dayMap);
  const dayVals = Object.values(dayMap);

  if (donutChart) donutChart.destroy();
  if (donutRef.value) {
    donutChart = new Chart(donutRef.value, {
      type: "doughnut",
      data: {
        labels: catLabels,
        datasets: [{ data: catVals, backgroundColor: ["#6366F1", "#8B5CF6", "#EC4899", "#F59E0B", "#10B981", "#3B82F6"], borderWidth: 0 }],
      },
      options: {
        maintainAspectRatio: false,
        cutout: "65%",
        plugins: { legend: { labels: { color: "#d1d5db" } } },
      },
    });
  }

  if (lineChart) lineChart.destroy();
  if (lineRef.value) {
    lineChart = new Chart(lineRef.value, {
      type: "line",
      data: {
        labels: dayLabels,
        datasets: [
          { label: "Spending", data: dayVals, borderColor: "#6366F1", backgroundColor: "rgba(99,102,241,0.2)", tension: 0.35, fill: true, pointRadius: 2 },
        ],
      },
      options: {
        maintainAspectRatio: false,
        scales: {
          x: { ticks: { color: "#9ca3af" }, grid: { color: "rgba(255,255,255,0.06)" } },
          y: { ticks: { color: "#9ca3af" }, grid: { color: "rgba(255,255,255,0.06)" } },
        },
        plugins: { legend: { labels: { color: "#d1d5db" } } },
      },
    });
  }
};

// Budgets
const budgetsLoading = ref(false);
const budgetCategory = ref("");
const budgetLimit = ref(null);
const budgetComparison = ref([]);

const fetchBudgets = async () => {
  budgetsLoading.value = true;
  try {
    const { data } = await compareBudgets();
    budgetComparison.value = data || [];
  } finally {
    budgetsLoading.value = false;
  }
};

const onSetBudget = async () => {
  if (!budgetCategory.value || !budgetLimit.value) {
    toast.warning("Please select category and enter a limit");
    return;
  }
  try {
    await upsertBudget({ category: budgetCategory.value.trim(), limit: Number(budgetLimit.value) });
    toast.success("Budget saved ✅");
    budgetCategory.value = "";
    budgetLimit.value = null;
    await fetchBudgets();
  } catch {
    toast.error("Failed to save budget");
  }
};

// Utils
const fmtDate = (d) => new Date(d).toLocaleDateString();
const fmtAmt = (n) => Number(n || 0).toFixed(2);

// Lifecycle
onMounted(async () => {
  await fetchExpenses();
  await fetchBudgets();
});
watch(() => expenses.value.length, updateCharts);
</script>

<style scoped>
.input-graphite {
  @apply px-4 py-3 rounded-xl bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}
.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.btn-graphite {
  @apply px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow-md hover:shadow-indigo-500/30 transition;
}
.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 text-center shadow-md hover:shadow-indigo-500/20 transition;
}
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-lg shadow-md p-6;
}
.panel-title {
  @apply text-white font-semibold mb-3;
}
.chart-wrap {
  @apply relative w-full h-[300px] md:h-[360px];
}
</style>
