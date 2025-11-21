<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 font-sans overflow-hidden">
    <!-- Particle BG -->
    <div class="particle-bg"></div>

    <!-- Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <div class="relative z-10 px-5 py-8 mx-auto max-w-7xl">
      <!-- Header -->
      <header class="mb-8 text-center">
        <h1 class="text-3xl md:text-4xl font-bold tracking-tight text-white">💼 My Dashboard</h1>
        <p class="text-gray-400 mt-2">Manage expenses, track budgets, and gain insights easily.</p>
      </header>

      <!-- Tabs -->
      <div class="flex justify-center mb-8 space-x-3">
        <button @click="activeTab = 'overview'" :class="tabClass('overview')">Overview</button>
        <button @click="activeTab = 'expenses'" :class="tabClass('expenses')">Expenses</button>
        <button @click="activeTab = 'budgets'" :class="tabClass('budgets')">Budgets</button>

        <button
          @click="() => { activeTab = 'notifications'; unseenCount = 0; }"
          :class="[tabClass('notifications'), 'relative']"
        >
          Notifications
          <span
            v-if="unseenCount > 0"
            class="absolute -top-2 -right-2 bg-red-600 text-white text-xs font-bold w-6 h-6 rounded-full flex items-center justify-center"
          >
            {{ unseenCount }}
          </span>
        </button>
      </div>

      <!-- ======================= -->
      <!--        OVERVIEW         -->
      <!-- ======================= -->
      <section v-if="activeTab === 'overview'">
        <!-- Range Selector -->
        <RangeSelector
          :start-date="rangeStart"
          :end-date="rangeEnd"
          @update:range="onRangeChange"
        />

        <!-- Summary cards (for selected range) -->
        <SummaryCards :summary="summaryData" />

        <!-- Calendar Heatmap (new month layout) -->
        <CalendarMonth :expenses="rangeExpenses" />

        <!-- Weekly trend (last 7 days, inside range) -->
        <WeeklyTrend :expenses="rangeExpenses" class="mt-6" />

        <!-- Budget Overview for range -->
        <BudgetOverview :budgets="budgets" :expenses="rangeExpenses" class="mt-6" />

        <!-- Category breakdown for range -->
        <CategoryMiniBreakdown :expenses="rangeExpenses" class="mt-6" />

        <!-- Insights for range -->
        <InsightsWidget :insights="insightMessages" class="mt-6" />
      </section>

      <!-- ======================= -->
      <!--        EXPENSES         -->
      <!-- ======================= -->
      <section v-else-if="activeTab === 'expenses'">
        <!-- Add expense form -->
        <div class="panel">
          <h2 class="panel-title">Add New Expense</h2>

          <form @submit.prevent="onAdd" class="grid grid-cols-1 md:grid-cols-5 gap-3 md:gap-4">
            <input
              v-model.trim="title"
              class="input-graphite md:col-span-2"
              placeholder="Title"
            />

            <select v-model="category" class="select-graphite">
              <option disabled value="">Select Category</option>
              <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
            </select>

            <input
              v-if="category === 'Other'"
              v-model.trim="customCategory"
              class="input-graphite"
              placeholder="Custom category"
            />

            <input
              v-model.number="amount"
              type="number"
              step="0.01"
              class="input-graphite"
              placeholder="Amount ($)"
            />

            <input v-model="date" type="date" class="input-graphite" />

            <div class="md:col-span-5 flex justify-end pt-2">
              <button type="submit" class="btn-graphite">➕ Add Expense</button>
            </div>
          </form>

          <p v-if="formError" class="text-rose-300 mt-3 text-sm">
            {{ formError }}
          </p>
        </div>

        <!-- Expense list -->
        <section class="panel">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-4">
            <h2 class="panel-title">Your Expenses</h2>

            <div class="flex gap-3">
              <select v-model="listFilter" class="select-graphite">
                <option value="daily">Daily</option>
                <option value="monthly">Monthly</option>
                <option value="all">All</option>
              </select>

              <input v-model.trim="search" class="select-graphite" placeholder="Search…" />
            </div>
          </div>

          <div v-if="loading" class="loading-text">Loading expenses…</div>

          <ul v-else-if="displayedExpenses.length" class="space-y-3">
            <li v-for="e in displayedExpenses" :key="e.id" class="expense-row">
              <div>
                <h3 class="font-semibold text-gray-100 truncate">{{ e.title }}</h3>
                <p class="text-xs text-gray-500 mt-0.5">
                  {{ e.category }} • {{ fmtDate(e.date) }}
                </p>
              </div>

              <div class="flex items-center gap-4">
                <span class="font-semibold text-indigo-400 text-lg">
                  ${{ fmtAmt(e.amount) }}
                </span>
                <button @click="onDelete(e.id)" class="delete-btn">🗑️</button>
              </div>
            </li>
          </ul>

          <p v-else class="loading-text">No expenses found.</p>
        </section>
      </section>

      <!-- ======================= -->
      <!--         BUDGETS         -->
      <!-- ======================= -->
      <section v-else-if="activeTab === 'budgets'">
        <div class="panel">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-6">
            <h2 class="panel-title">Your Budgets</h2>
            <button @click="openCreateBudget = true" class="btn-graphite">➕ Add Budget</button>
          </div>

          <div v-if="budgetsLoading" class="loading-text">Loading budgets…</div>

          <div v-else-if="budgets.length" class="grid md:grid-cols-2 gap-4">
            <div v-for="b in budgets" :key="b.id" class="panel-box relative">
              <div class="flex justify-between mb-2">
                <h3 class="font-semibold text-gray-100">{{ b.category }}</h3>

                <div class="flex gap-3 items-center">
                  <button
                    @click="openEdit(b)"
                    class="text-yellow-400 hover:text-yellow-300 text-sm"
                  >
                    ✏️
                  </button>
                  <button
                    @click="onDeleteBudget(b.id)"
                    class="text-red-400 hover:text-red-300 text-sm"
                  >
                    🗑️
                  </button>
                </div>
              </div>

              <BudgetBar :percentage="getPercent(b)" class="mb-3" />

              <p class="text-gray-400 text-sm leading-relaxed">
                Spent:
                <span class="text-indigo-400">${{ fmtAmt(getSpent(b.category)) }}</span>
                /
                Limit:
                <span class="text-gray-200">${{ fmtAmt(b.limit) }}</span>
                <br />
                Remaining:
                <span :class="getRemaining(b) < 0 ? 'text-red-400' : 'text-green-400'">
                  ${{ fmtAmt(getRemaining(b)) }}
                </span>
              </p>
            </div>
          </div>

          <p v-else class="loading-text">No budgets found — add one above 👆</p>
        </div>
      </section>

      <!-- ======================= -->
      <!--      NOTIFICATIONS      -->
      <!-- ======================= -->
      <section v-else-if="activeTab === 'notifications'">
        <div class="panel">
          <h2 class="panel-title">Notifications</h2>

          <p v-if="notifications.length === 0" class="loading-text">
            No notifications yet.
          </p>

          <ul v-else class="space-y-4">
            <li
              v-for="(n, i) in notifications"
              :key="i"
              :class="[
                'p-4 rounded-xl border bg-[#26292E]/60 shadow flex justify-between items-center',
                n.type === 'danger'
                  ? 'border-red-500/40'
                  : n.type === 'warning'
                    ? 'border-yellow-500/40'
                    : 'border-[#35383E]'
              ]"
            >
              <div>
                <p
                  :class="[
                    'font-semibold',
                    n.type === 'danger'
                      ? 'text-red-400'
                      : n.type === 'warning'
                        ? 'text-yellow-400'
                        : 'text-gray-300'
                  ]"
                >
                  {{ n.message }}
                </p>
                <p class="text-gray-500 text-xs mt-1">{{ fmtDate(n.date) }}</p>
              </div>
            </li>
          </ul>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import api from "../api";

/* Overview components */
import SummaryCards from "../components/overview/SummaryCards.vue";
import RangeSelector from "../components/overview/RangeSelector.vue";
import WeeklyTrend from "../components/overview/WeeklyTrend.vue";
import CategoryMiniBreakdown from "../components/overview/CategoryMiniBreakdown.vue";
import InsightsWidget from "../components/overview/InsightsWidget.vue";
import BudgetOverview from "../components/overview/BudgetOverview.vue";
import CalendarMonth from "../components/overview/CalendarMonth.vue";

import BudgetBar from "../components/BudgetBar.vue";

/* Tabs */
const activeTab = ref("overview");

/* Core data */
const expenses = ref([]);
const loading = ref(false);

const budgets = ref([]);
const budgetsLoading = ref(false);

const notifications = ref([]);
const unseenCount = ref(0);

/* Category options for expense form */
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
  "Other",
]);

/* -------- Date range for overview -------- */
const rangeStart = ref("");
const rangeEnd = ref("");

function onRangeChange({ start, end }) {
  rangeStart.value = start;
  rangeEnd.value = end;
}

/* Expenses + budgets fetch */
async function fetchExpenses() {
  loading.value = true;
  try {
    const { data } = await api.get("/api/expense");
    expenses.value = data || [];
  } catch (err) {
    console.error("Failed to fetch expenses", err);
  } finally {
    loading.value = false;
  }
}

async function fetchBudgets() {
  budgetsLoading.value = true;
  try {
    const { data } = await api.get("/api/budget");
    budgets.value = data || [];
  } catch (err) {
    console.error("Failed to fetch budgets", err);
  } finally {
    budgetsLoading.value = false;
  }
}

/* -------- Expenses filtered by range (for overview) -------- */
const rangeExpenses = computed(() => {
  if (!rangeStart.value || !rangeEnd.value) return expenses.value;

  const start = new Date(rangeStart.value);
  const end = new Date(rangeEnd.value);
  if (isNaN(start) || isNaN(end)) return expenses.value;

  end.setHours(23, 59, 59, 999);

  return expenses.value.filter((e) => {
    if (!e || !e.date) return false;
    const d = new Date(e.date);
    return d >= start && d <= end;
  });
});

/* -------- Summary data (Today / This month / Total in range) -------- */
const summaryData = computed(() => {
  const list = rangeExpenses.value || [];
  const now = new Date();
  const todayStr = now.toDateString();

  let total = 0;
  let today = 0;
  let thisMonth = 0;

  for (const e of list) {
    if (!e || !e.date) continue;
    const d = new Date(e.date);
    const amt = Number(e.amount || 0);

    total += amt;
    if (d.toDateString() === todayStr) today += amt;
    if (d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear()) {
      thisMonth += amt;
    }
  }

  return { total, today, month: thisMonth };
});

/* -------- Insights for selected range -------- */
const insightMessages = computed(() => {
  const list = rangeExpenses.value || [];
  const total = summaryData.value.total || 0;
  const today = summaryData.value.today || 0;

  const insights = [];

  if (!list.length) {
    insights.push("No spending detected in this date range yet.");
    return insights;
  }

  if (total > 1000) {
    insights.push("⚠️ High overall spending in this range (over $1,000).");
  }

  if (today > 200) {
    insights.push("💡 You spent more than $200 today.");
  }

  // Simple average daily spending in range
  const daysSet = new Set(
    list
      .filter((e) => e && e.date)
      .map((e) => new Date(e.date).toISOString().slice(0, 10))
  );
  const avgPerDay = daysSet.size ? total / daysSet.size : 0;

  if (avgPerDay > 50) {
    insights.push("📈 Your average daily spending in this range is above $50.");
  }

  if (!insights.length) {
    insights.push("✅ Your spending looks steady in this date range.");
  }

  return insights;
});

/* ====================== */
/*  EXPENSES TAB LOGIC    */
/* ====================== */
const title = ref("");
const category = ref("");
const customCategory = ref("");
const amount = ref(null);
const date = ref("");
const formError = ref("");

const search = ref("");
const listFilter = ref("all"); // 'all' | 'daily' | 'monthly'

const displayedExpenses = computed(() => {
  let list = [...expenses.value];

  // Basic time filters
  if (listFilter.value === "daily") {
    const today = new Date().toDateString();
    list = list.filter((e) => new Date(e.date).toDateString() === today);
  } else if (listFilter.value === "monthly") {
    const now = new Date();
    list = list.filter((e) => {
      const d = new Date(e.date);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    });
  }

  // Search filter
  if (search.value.trim()) {
    const s = search.value.toLowerCase();
    list = list.filter(
      (e) =>
        (e.title && e.title.toLowerCase().includes(s)) ||
        (e.category && e.category.toLowerCase().includes(s))
    );
  }

  // Newest first
  return list.sort((a, b) => new Date(b.date) - new Date(a.date));
});

async function onAdd() {
  formError.value = "";

  if (!title.value || !category.value || !amount.value || !date.value) {
    formError.value = "Please fill in all fields.";
    return;
  }

  try {
    await api.post("/api/expense", {
      title: title.value.trim(),
      category:
        category.value === "Other"
          ? customCategory.value.trim()
          : category.value.trim(),
      amount: Number(amount.value),
      date: date.value,
    });

    // clear form
    title.value = "";
    category.value = "";
    customCategory.value = "";
    amount.value = null;
    date.value = "";

    await fetchExpenses();
  } catch (err) {
    console.error("Failed to add expense", err);
    formError.value = "Failed to add expense. Please try again.";
  }
}

async function onDelete(id) {
  try {
    await api.delete(`/api/expense/${id}`);
    await fetchExpenses();
  } catch (err) {
    console.error("Failed to delete expense", err);
  }
}

/* ====================== */
/*   BUDGETS TAB LOGIC    */
/* ====================== */

function getSpent(cat) {
  return expenses.value
    .filter((e) => e.category === cat)
    .reduce((sum, e) => sum + Number(e.amount || 0), 0);
}

function getRemaining(b) {
  return (b.limit || 0) - getSpent(b.category);
}

function getPercent(b) {
  const spent = getSpent(b.category);
  if (!b.limit) return 0;
  return (spent / b.limit) * 100;
}

const openCreateBudget = ref(false);
const openEditBudget = ref(false);
const editBudgetData = ref({ id: null, category: "", limit: null });

async function createBudget() {
  if (!editBudgetData.value.category || !editBudgetData.value.limit) return;

  try {
    await api.post("/api/budget", {
      category: editBudgetData.value.category.trim(),
      limit: Number(editBudgetData.value.limit),
    });
    openCreateBudget.value = false;
    editBudgetData.value = { id: null, category: "", limit: null };
    await fetchBudgets();
  } catch (err) {
    console.error("Failed to create budget", err);
  }
}

// Used by "Add Budget" button – open empty modal
watch(openCreateBudget, (val) => {
  if (val) {
    editBudgetData.value = { id: null, category: "", limit: null };
  }
});

function openEdit(b) {
  editBudgetData.value = { ...b };
  openEditBudget.value = true;
}

async function saveEditedBudget() {
  const b = editBudgetData.value;
  if (!b.category || !b.limit) return;

  try {
    await api.put(`/api/budget/${b.id}`, {
      category: b.category.trim(),
      limit: Number(b.limit),
    });
    openEditBudget.value = false;
    editBudgetData.value = { id: null, category: "", limit: null };
    await fetchBudgets();
  } catch (err) {
    console.error("Failed to update budget", err);
  }
}

async function onDeleteBudget(id) {
  try {
    await api.delete(`/api/budget/${id}`);
    await fetchBudgets();
  } catch (err) {
    console.error("Failed to delete budget", err);
  }
}

/* ====================== */
/*   UTIL + MOUNT LOGIC   */
/* ====================== */

function fmtDate(d) {
  return new Date(d).toLocaleDateString();
}
function fmtAmt(n) {
  return Number(n || 0).toFixed(2);
}
function tabClass(t) {
  return [
    "px-5 py-2 rounded-lg font-semibold transition",
    activeTab.value === t
      ? "bg-indigo-600 text-white shadow-md"
      : "bg-[#1D1F22] text-gray-400 hover:text-white",
  ].join(" ");
}

onMounted(async () => {
  await fetchExpenses();
  await fetchBudgets();

  // Default range = last 30 days
  const now = new Date();
  const start = new Date(now);
  start.setDate(start.getDate() - 29);
  rangeStart.value = start.toISOString().slice(0, 10);
  rangeEnd.value = now.toISOString().slice(0, 10);
});
</script>

<style scoped>
/* Panels & cards used in this view */
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-6 md:p-8 mb-8;
}

.panel-title {
  @apply text-xl font-semibold text-white mb-5;
}

.panel-box {
  @apply mb-5 p-4 rounded-xl border border-[#35383E] bg-[#26292E]/70 shadow-md hover:shadow-indigo-500/20 transition;
}

/* Inputs */
.input-graphite {
  @apply px-4 py-3 rounded-xl bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}

.btn-graphite {
  @apply px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow-md hover:shadow-indigo-500/30 transition;
}

/* Loading text */
.loading-text {
  @apply text-center text-gray-500 py-6;
}

/* Expenses list */
.expense-row {
  @apply flex justify-between items-center bg-[#26292E]/80 rounded-lg px-4 py-3 border border-[#35383E] hover:border-indigo-500 transition;
}

.delete-btn {
  @apply text-gray-500 hover:text-red-500 transition;
}
</style>
