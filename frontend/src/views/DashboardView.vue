<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 font-sans overflow-hidden">
    <!-- 🌌 Particle Background -->
    <div class="particle-bg"></div>

    <!-- 🌈 Gradient Overlay -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <!-- 💸 Dashboard Content -->
    <div class="relative z-10 px-5 py-8 mx-auto max-w-7xl">
      <!-- Header -->
      <header class="mb-8 text-center">
        <h1 class="text-3xl md:text-4xl font-bold tracking-tight text-white">
          💼 My Dashboard
        </h1>
        <p class="text-gray-400 mt-2">Manage expenses, track budgets, and gain insights easily.</p>
      </header>

      <!-- Tabs -->
      <div class="flex justify-center mb-8 space-x-3">
        <!-- Overview -->
        <button @click="activeTab = 'overview'" :class="tabClass('overview')">Overview</button>

        <!-- Expenses -->
        <button @click="activeTab = 'expenses'" :class="tabClass('expenses')">Expenses</button>

        <!-- Budgets -->
        <button @click="activeTab = 'budgets'" :class="tabClass('budgets')">Budgets</button>

        <!-- Notifications -->
        <button @click="activeTab = 'notifications'; unseenCount = 0" :class="[tabClass('notifications'), 'relative']">
          Notifications
          <span v-if="unseenCount > 0" class="absolute -top-2 -right-2 bg-red-600 text-white text-xs font-bold
                   w-6 h-6 rounded-full flex items-center justify-center">
            {{ unseenCount }}
          </span>
        </button>
      </div>

      <!-- ============================ -->
      <!-- 🏠 OVERVIEW TAB -->
      <!-- ============================ -->
      <section v-if="activeTab === 'overview'">
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

        <!-- Budget Overview -->
        <section class="panel">
          <h2 class="panel-title">Budget Overview</h2>

          <div v-if="budgetsLoading" class="loading-text">Loading budgets…</div>

          <div v-else-if="mergedBudgets.length">
            <div v-for="b in mergedBudgets" :key="b.category" class="panel-box">
              <div class="flex justify-between items-start mb-2">
                <div>
                  <h3 class="font-semibold text-gray-100">{{ b.category }}</h3>
                  <p class="text-[11px] text-gray-500">
                    Limit: ${{ fmtAmt(b.limit) }}
                  </p>
                </div>

                <div class="flex flex-col items-end gap-1">
                  <span :class="[
                    'text-sm font-semibold',
                    b.percentage > 100 ? 'text-red-400' : 'text-green-400'
                  ]">
                    {{ b.percentage.toFixed(1) }}%
                  </span>
                  <!-- small edit/delete for quick access -->
                  <div class="flex gap-2 text-[11px]">
                    <button @click="startEditBudget(b)" class="text-gray-400 hover:text-indigo-400">
                      Edit
                    </button>
                    <button @click="removeBudget(b)" class="text-gray-500 hover:text-red-400">
                      Delete
                    </button>
                  </div>
                </div>
              </div>

              <BudgetBar :percentage="b.percentage" class="mb-3" />

              <p class="text-gray-400 text-sm">
                Spent:
                <span class="text-indigo-400">${{ fmtAmt(b.spent) }}</span> /
                Limit:
                <span class="text-gray-200">${{ fmtAmt(b.limit) }}</span><br />

                Remaining:
                <span :class="b.remaining < 0 ? 'text-red-400' : 'text-green-400'">
                  ${{ fmtAmt(b.remaining) }}
                </span>
              </p>
            </div>
          </div>

          <p v-else class="loading-text">No budgets set yet. Add one in the Budgets tab 👆</p>
        </section>

        <!-- Recent Transactions -->
        <section class="panel">
          <h2 class="panel-title">Recent Transactions</h2>
          <div v-if="loading" class="loading-text">Loading…</div>

          <ul v-else-if="recentExpenses.length">
            <li v-for="e in recentExpenses" :key="e.id" class="transaction-row">
              <span>{{ e.title }} ({{ e.category }})</span>
              <span class="text-indigo-400">${{ fmtAmt(e.amount) }}</span>
            </li>
          </ul>

          <p v-else class="loading-text">No recent expenses found.</p>
        </section>

        <!-- Smart Insights -->
        <section class="card-graphite text-center">
          <h3 class="text-xl font-semibold mb-2">💡 Smart Insight</h3>
          <p class="text-gray-300">{{ insightMessage }}</p>
        </section>
      </section>

      <!-- ============================ -->
      <!-- 💰 EXPENSES TAB -->
      <!-- ============================ -->
      <section v-else-if="activeTab === 'expenses'">
        <section class="panel">
          <h2 class="panel-title">Add New Expense</h2>

          <form @submit.prevent="onAdd" class="grid grid-cols-1 md:grid-cols-5 gap-3 md:gap-4">
            <input v-model.trim="title" class="input-graphite md:col-span-2" placeholder="Title" />

            <select v-model="category" class="select-graphite">
              <option disabled value="">Select Category</option>
              <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
            </select>

            <input v-if="category === 'Other'" v-model.trim="customCategory" class="input-graphite"
              placeholder="Custom category" />

            <input v-model.number="amount" type="number" step="0.01" class="input-graphite" placeholder="Amount ($)" />

            <input v-model="date" type="date" class="input-graphite" />

            <div class="md:col-span-5 flex justify-end pt-2">
              <button type="submit" class="btn-graphite">➕ Add Expense</button>
            </div>
          </form>

          <p v-if="formError" class="text-rose-300 mt-3 text-sm">{{ formError }}</p>
        </section>

        <!-- Expense List -->
        <section class="panel">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-4">
            <h2 class="panel-title">Your Expenses</h2>

            <div class="flex gap-3">
              <select v-model="filter" class="select-graphite">
                <option value="daily">Daily</option>
                <option value="monthly">Monthly</option>
                <option value="all">All</option>
              </select>

              <input v-model.trim="search" class="select-graphite" placeholder="Search…" />
            </div>
          </div>

          <div v-if="loading" class="loading-text">Loading expenses…</div>

          <ul v-else-if="filteredExpenses.length" class="space-y-3">
            <li v-for="e in filteredExpenses" :key="e.id" class="expense-row">
              <div>
                <h3 class="font-semibold text-gray-100 truncate">{{ e.title }}</h3>
                <p class="text-xs text-gray-500 mt-0.5">{{ e.category }} • {{ fmtDate(e.date) }}</p>
              </div>

              <div class="flex items-center gap-4">
                <span class="font-semibold text-indigo-400 text-lg">${{ fmtAmt(e.amount) }}</span>
                <button @click="onDelete(e.id)" class="delete-btn">🗑️</button>
              </div>
            </li>
          </ul>

          <p v-else class="loading-text">No expenses found.</p>
        </section>
      </section>

      <!-- ============================ -->
      <!-- 📊 BUDGET TAB -->
      <!-- ============================ -->
      <section v-else-if="activeTab === 'budgets'">
        <div class="panel">
          <!-- Header -->
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-6">
            <h2 class="panel-title">Your Budgets</h2>

            <button @click="openCreateBudget = true" class="btn-graphite">➕ Add Budget</button>
          </div>

          <!-- Loading -->
          <div v-if="budgetsLoading" class="loading-text">Loading budgets…</div>

          <!-- Budget List -->
          <div v-else-if="budgets.length" class="grid md:grid-cols-2 gap-4">
            <div v-for="b in budgets" :key="b.id" class="panel-box relative">
              <!-- Category + Actions -->
              <div class="flex justify-between mb-2">
                <h3 class="font-semibold text-gray-100">{{ b.category }}</h3>

                <div class="flex gap-3 items-center">
                  <!-- Edit -->
                  <button @click="openEdit(b)" class="text-yellow-400 hover:text-yellow-300 text-sm">✏️</button>

                  <!-- Delete -->
                  <button @click="onDeleteBudget(b.id, b.category)"
                    class="text-red-400 hover:text-red-300 text-sm">🗑️</button>
                </div>
              </div>

              <!-- Progress Bar -->
              <BudgetBar :percentage="getPercent(b)" class="mb-3" />

              <!-- Stats -->
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


      <!-- ============================ -->
      <!-- 🔔 NOTIFICATIONS TAB -->
      <!-- ============================ -->
      <section v-else-if="activeTab === 'notifications'">
        <div class="panel">
          <h2 class="panel-title">Notifications</h2>

          <p v-if="notifications.length === 0" class="loading-text">
            No notifications yet.
          </p>

          <ul v-else class="space-y-4">
            <li v-for="(n, i) in notifications" :key="i" :class="[
              'p-4 rounded-xl border bg-[#26292E]/60 shadow flex justify-between items-center',
              n.type === 'danger' ? 'border-red-500/40' :
                n.type === 'warning' ? 'border-yellow-500/40' :
                  'border-[#35383E]'
            ]">
              <div>
                <p :class="[
                  'font-semibold',
                  n.type === 'danger' ? 'text-red-400' :
                    n.type === 'warning' ? 'text-yellow-400' :
                      'text-gray-300'
                ]">
                  {{ n.message }}
                </p>
                <p class="text-gray-500 text-xs mt-1">{{ fmtDate(n.date) }}</p>
              </div>
            </li>
          </ul>
        </div>
      </section>
    </div>

    <!-- ⭐ Floating Add Expense Button -->
    <button @click="openAddModal = true" class="fab">
      +
    </button>

    <!-- ⭐ Add Expense Modal -->
    <div v-if="openAddModal" class="modal-overlay">
      <div class="modal-box">
        <button @click="openAddModal = false" class="modal-close">✖</button>

        <h2 class="panel-title mb-5">Add New Expense</h2>

        <form @submit.prevent="onAdd" class="space-y-4">
          <input v-model.trim="title" class="input-graphite w-full" placeholder="Title" />

          <select v-model="category" class="select-graphite w-full">
            <option disabled value="">Select Category</option>
            <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
          </select>

          <input v-if="category === 'Other'" v-model.trim="customCategory" class="input-graphite w-full"
            placeholder="Custom category" />

          <input v-model.number="amount" type="number" class="input-graphite w-full" placeholder="Amount ($)" />

          <input v-model="date" type="date" class="input-graphite w-full" />

          <button type="submit" class="btn-graphite w-full">Save Expense</button>
        </form>
      </div>
    </div>

    <!-- ⭐ CREATE BUDGET MODAL -->
    <div v-if="openCreateBudget" class="modal-overlay">
      <div class="modal-box">
        <button class="modal-close" @click="openCreateBudget = false">✖</button>

        <h2 class="panel-title mb-5">Create Budget</h2>

        <div class="space-y-4">
          <select v-model="budgetCategory" class="select-graphite w-full">
            <option disabled value="">Select Category</option>
            <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
          </select>

          <input v-model.number="budgetLimit" type="number" class="input-graphite w-full" placeholder="Limit ($)" />

          <button @click="onCreateBudget" class="btn-graphite w-full">Save</button>
        </div>
      </div>
    </div>

    <!-- ⭐ EDIT BUDGET MODAL -->
    <div v-if="openEditBudget" class="modal-overlay">
      <div class="modal-box">
        <button class="modal-close" @click="openEditBudget = false">✖</button>

        <h2 class="panel-title mb-5">Edit Budget</h2>

        <div class="space-y-4">
          <input v-model="editBudgetData.category" class="input-graphite w-full" placeholder="Category" />

          <input v-model.number="editBudgetData.limit" type="number" class="input-graphite w-full"
            placeholder="Limit ($)" />

          <button @click="onEditBudget" class="btn-graphite w-full">Update</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onBeforeUnmount } from "vue";
import { useToast } from "vue-toastification";
import api from "../api";
import BudgetBar from "../components/BudgetBar.vue";

const toast = useToast();

// Tabs
const activeTab = ref("overview");

// Modal
const openAddModal = ref(false);

// Edit budget modal
const showEditModal = ref(false);
const editingBudget = ref(null); // full budget object from GET /api/budget/{id}
const editingCategory = ref("");
const editingLimit = ref(null);

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
  "Other",
]);

// Expense Inputs
const title = ref("");
const category = ref("");
const customCategory = ref("");
const amount = ref(null);
const date = ref("");
const formError = ref("");

// Data
const expenses = ref([]);
const loading = ref(false);

// Budgets (raw + comparison)
const budgetsLoading = ref(false);
const budgets = ref([]);             // from GET /api/budget
const budgetComparison = ref([]);    // from GET /api/budget/compare
const budgetCategory = ref("");
const budgetLimit = ref(null);
const openCreateBudget = ref(false);
const openEditBudget = ref(false);
const editBudgetData = ref({ id: null, category: "", limit: null });


// Notifications
const notifications = ref([]);
const unseenCount = ref(0);

// ===== Rotating subtitle for hero (optional but nice) =====
const rotatingWords = [
  "Track. Budget. Grow.",
  "See where every dollar goes.",
  "Stay financially confident.",
  "Turn spending into insight.",
  "Build smarter money habits.",
];

const currentWord = ref(rotatingWords[0]);
let wordIndex = 0;
let intervalId = null;

onMounted(async () => {
  intervalId = setInterval(() => {
    wordIndex = (wordIndex + 1) % rotatingWords.length;
    currentWord.value = rotatingWords[wordIndex];
  }, 2500);

  await fetchExpenses();
  await fetchBudgets();
});

onBeforeUnmount(() => {
  if (intervalId) clearInterval(intervalId);
});

// ===== Budget Notifications =====
const generateNotifications = () => {
  notifications.value = [];

  for (const b of mergedBudgets.value) {
    if (b.percentage > 100) {
      const msg = `${b.category} budget exceeded by $${fmtAmt(Math.abs(b.remaining))}!`;
      notifications.value.push({
        type: "danger",
        message: msg,
        date: new Date(),
      });
      toast.error(msg + " 🚨");
    } 
    
    else if (b.percentage > 80) {
      const msg = `${b.category} budget is at ${b.percentage.toFixed(1)}% of its limit.`;
      notifications.value.push({
        type: "warning",
        message: msg,
        date: new Date(),
      });
      toast.warning(msg + " ⚠️");
    }
  }

  unseenCount.value = notifications.value.length;
};


watch(budgetComparison, () => generateNotifications());

// ===== API CALLS =====

// GET /api/budget/compare
const compareBudgets = () => {
  return api.get("/api/budget/compare");
};

// Expenses
const fetchExpenses = async () => {
  loading.value = true;
  try {
    const { data } = await api.get("/api/expense");
    expenses.value = data || [];
  } finally {
    loading.value = false;
  }
};

// Budgets: GET /api/budget + GET /api/budget/compare
const fetchBudgets = async () => {
  budgetsLoading.value = true;
  try {
    const { data } = await api.get("/api/budget"); // all budgets
    budgets.value = data || [];

    // also recalc comparison table
    const compare = await compareBudgets();
    budgetComparison.value = compare.data?.length ? compare.data : budgets.value.map(b => ({
      category: b.category,
      spent: 0,
      remaining: b.limit,
      percentage: 0
    }));

  } finally {
    budgetsLoading.value = false;
  }
};


const getSpent = (cat) => {
  const match = budgetComparison.value.find(x => x.category === cat);
  return match ? match.spent : 0;
};

const getRemaining = (b) => {
  return b.limit - getSpent(b.category);
};

const getPercent = (b) => {
  const spent = getSpent(b.category);
  return spent === 0 ? 0 : (spent / (b.limit || 1)) * 100;
};


// ===== Computed =====
const filter = ref("all");
const search = ref("");

const filteredExpenses = computed(() => {
  let list = expenses.value;

  if (filter.value === "daily") {
    const today = new Date().toDateString();
    list = list.filter((e) => new Date(e.date).toDateString() === today);
  } else if (filter.value === "monthly") {
    const now = new Date();
    list = list.filter((e) => {
      const d = new Date(e.date);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    });
  }

  if (search.value.trim()) {
    const s = search.value.toLowerCase();
    list = list.filter(
      (e) =>
        e.title.toLowerCase().includes(s) ||
        e.category.toLowerCase().includes(s)
    );
  }

  return list;
});

// join comparison with raw budgets so each card has its Id
const mergedBudgets = computed(() =>
  budgetComparison.value.map((b) => {
    const match = budgets.value.find(
      (raw) => raw.category === b.category
    );
    return {
      ...b,
      id: match ? match.id : null,
      limit: match ? match.limit : 0,
    };
  })
);

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

const recentExpenses = computed(() =>
  expenses.value
    .slice()
    .sort((a, b) => new Date(b.date) - new Date(a.date))
    .slice(0, 5)
);

const insightMessage = computed(() => {
  if (summary.value.month > 1000)
    return "⚠️ You’ve spent over $1,000 this month!";
  if (mergedBudgets.value.some((b) => b.percentage > 100))
    return "🚨 You’ve exceeded some budget limits.";
  if (mergedBudgets.value.some((b) => b.percentage > 80))
    return "⚠️ Some categories are nearing their limits.";
  return "✅ You’re on track with your spending goals. Great job!";
});

// ===== Expense Actions =====
const onAdd = async () => {
  if (!title.value || !category.value || !amount.value || !date.value) {
    toast.warning("Please fill all fields");
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

    toast.success("Expense added 💰");

    title.value = "";
    category.value = "";
    customCategory.value = "";
    amount.value = null;
    date.value = "";

    openAddModal.value = false;

    await fetchExpenses();
    await fetchBudgets();   // 👈 auto-refresh budgets after adding an expense

  } catch {
    toast.error("Failed to add expense");
  }
};

const onDelete = async (id) => {
  try {
    await api.delete(`/api/expense/${id}`);
    toast("Expense deleted 🗑️", { type: "info" });
    await fetchExpenses();
  } catch {
    toast.error("Failed to delete expense");
  }
};

// ===== Budget Actions =====

// POST /api/budget (create or update by category)
const onCreateBudget = async () => {
  if (!budgetCategory.value || !budgetLimit.value) {
    toast.warning("Please enter category and limit");
    return;
  }

  try {
    await api.post("/api/budget", {
      category: budgetCategory.value.trim(),
      limit: Number(budgetLimit.value),
    });

    toast.success("Budget added");

    budgetCategory.value = "";
    budgetLimit.value = null;
    openCreateBudget.value = false;

    await fetchBudgets();
  } catch {
    toast.error("Failed to add budget");
  }
};


// GET /api/budget/{id} then open modal
const onEditBudget = async () => {
  const b = editBudgetData.value;

  if (!b.category || !b.limit) {
    toast.warning("Please fill all fields");
    return;
  }

  try {
    await api.put(`/api/budget/${b.id}`, {
      category: b.category.trim(),
      limit: Number(b.limit),
    });

    toast.success("Budget updated");

    openEditBudget.value = false;
    await fetchBudgets();
  } catch (err) {
    toast.error(err.response?.data || "Failed to update");
  }
};


const closeEditModal = () => {
  showEditModal.value = false;
  editingBudget.value = null;
  editingCategory.value = "";
  editingLimit.value = null;
};

// PUT /api/budget/{id}
const saveEditBudget = async () => {
  if (!editingBudget.value) return;

  if (!editingCategory.value || !editingLimit.value) {
    toast.warning("Please provide category and limit");
    return;
  }

  try {
    if (editingBudget.value.id) {
      await api.put(`/api/budget/${editingBudget.value.id}`, {
        category: editingCategory.value.trim(),
        limit: Number(editingLimit.value),
      });
    } else {
      // if no id, just reuse POST logic
      await api.post("/api/budget", {
        category: editingCategory.value.trim(),
        limit: Number(editingLimit.value),
      });
    }

    toast.success("Budget updated");
    closeEditModal();
    await fetchBudgets();
  } catch (err) {
    toast.error("Failed to update budget");
  }
};

const onDeleteBudget = async (id, category) => {
  try {
    await api.delete(`/api/budget/${id}`);
    toast.info(`Budget "${category}" removed`);
    await fetchBudgets();
  } catch (err) {
    toast.error("Failed to delete budget");
  }
};

const openEdit = (b) => {
  editBudgetData.value = { ...b };
  openEditBudget.value = true;
};


// ===== Utils =====
const fmtDate = (d) => new Date(d).toLocaleDateString();
const fmtAmt = (n) => Number(n || 0).toFixed(2);

const tabClass = (t) =>
  [
    "px-5 py-2 rounded-lg font-semibold transition",
    activeTab.value === t
      ? "bg-indigo-600 text-white shadow-md"
      : "bg-[#1D1F22] text-gray-400 hover:text-white",
  ].join(" ");
</script>

<style scoped>
/* Panels */
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-6 md:p-8 mb-8;
}

.panel-title {
  @apply text-xl font-semibold text-white mb-5;
}

.panel-box {
  @apply mb-5 p-4 rounded-xl border border-[#35383E] bg-[#26292E]/70 shadow-md hover:shadow-indigo-500/20 transition;
}

.loading-text {
  @apply text-center text-gray-500 py-6;
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

/* Rows */
.transaction-row,
.expense-row {
  @apply flex justify-between items-center bg-[#26292E]/80 rounded-lg px-4 py-3 border border-[#35383E] hover:border-indigo-500 transition;
}

/* Delete Button */
.delete-btn {
  @apply text-gray-500 hover:text-red-500 transition;
}

/* Floating button */
.fab {
  @apply fixed bottom-6 right-6 bg-indigo-600 hover:bg-indigo-500 text-white w-14 h-14 rounded-full shadow-xl flex items-center justify-center text-3xl transition z-50;
}

/* Modal */
.modal-overlay {
  @apply fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-[60];
}

.modal-box {
  @apply bg-[#1D1F22] border border-[#35383E] p-6 rounded-2xl w-full max-w-md shadow-xl relative;
}

.modal-close {
  @apply absolute top-3 right-3 text-gray-400 hover:text-white text-xl;
}

/* Card */
.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 text-center shadow-md hover:shadow-indigo-500/20 transition;
}
</style>
