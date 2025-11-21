<template>
  <div class="panel rounded-2xl p-6 bg-[#1D1F22]/80 border border-[#35383E] shadow-lg">
    <h2 class="text-xl font-semibold text-white mb-4">
      💰 Budget Overview
    </h2>

    <!-- No budgets -->
    <p v-if="budgets.length === 0" class="text-gray-500 text-center py-6">
      No budgets created yet.
    </p>

    <!-- Content -->
    <div v-else>
      <!-- Totals -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <div class="summary-box">
          <p class="label">Total Budget</p>
          <p class="value">${{ fmt(totalBudget) }}</p>
        </div>

        <div class="summary-box">
          <p class="label">Spent (in range)</p>
          <p class="value">${{ fmt(totalSpent) }}</p>
        </div>

        <div class="summary-box">
          <p class="label">Remaining</p>
          <p class="value"
             :class="remaining < 0 ? 'text-red-400' : 'text-green-400'">
            ${{ fmt(remaining) }}
          </p>
        </div>
      </div>

      <!-- Progress bar -->
      <div class="w-full h-4 bg-[#35383E] rounded-lg overflow-hidden mb-8">
        <div
          class="h-full bg-indigo-500 transition-all"
          :style="{ width: percentUsed + '%' }"
        ></div>
      </div>

      <p class="text-sm text-gray-400 mb-6">
        You've used <span class="text-indigo-300 font-semibold">{{ percentUsed.toFixed(1) }}%</span>
        of your total budget.
      </p>

      <!-- Category list -->
      <div class="space-y-4">
        <div
          v-for="b in budgets"
          :key="b.id"
          class="cat-row"
        >
          <div class="flex-1">
            <p class="font-semibold text-gray-100">{{ b.category }}</p>

            <div class="h-2 bg-[#35383E] rounded-lg overflow-hidden mt-2">
              <div
                class="h-full bg-indigo-500"
                :style="{ width: getPercent(b) + '%' }"
              ></div>
            </div>

            <p class="text-xs text-gray-400 mt-1">
              ${{ fmt(getSpent(b.category)) }} / ${{ fmt(b.limit) }}
              <span
                :class="getRemaining(b) < 0 ? 'text-red-400' : 'text-green-400'"
              >
                ({{ getRemaining(b) < 0 ? 'Over by ' : 'Remaining ' }}
                ${{ fmt(Math.abs(getRemaining(b))) }})
              </span>
            </p>
          </div>

          <!-- Status Icon -->
          <div class="w-12 text-right text-lg"
               :class="getRemaining(b) < 0 ? 'text-red-400' : 'text-green-400'">
            {{ getRemaining(b) < 0 ? '⚠️' : '✔️' }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  budgets: {
    type: Array,
    default: () => [],
  },
  expenses: {
    type: Array,
    default: () => [],
  },
});

// Safe arrays
const exp = computed(() =>
  Array.isArray(props.expenses) ? props.expenses : []
);

const buds = computed(() =>
  Array.isArray(props.budgets) ? props.budgets : []
);

// ----- PER-CATEGORY HELPERS -----
function getSpent(cat) {
  return exp.value
    .filter((e) => e && e.category === cat)
    .reduce((sum, e) => sum + Number(e.amount || 0), 0);
}

function getRemaining(b) {
  const spent = getSpent(b.category);
  return Number(b.limit || 0) - spent;
}

function getPercent(b) {
  const limit = Number(b.limit || 0);
  if (limit <= 0) return 0;
  const spent = getSpent(b.category);
  return Math.min((spent / limit) * 100, 100);
}

// ----- TOTALS ACROSS ALL BUDGETS -----
const totalBudget = computed(() =>
  buds.value.reduce((s, b) => s + Number(b.limit || 0), 0)
);

const totalSpent = computed(() =>
  exp.value.reduce((s, e) => s + Number(e.amount || 0), 0)
);

const remaining = computed(() => totalBudget.value - totalSpent.value);

const percentUsed = computed(() => {
  if (totalBudget.value <= 0) return 0;
  return Math.min((totalSpent.value / totalBudget.value) * 100, 999);
});

const fmt = (v) => Number(v || 0).toFixed(2);
</script>



<style scoped>
.summary-box {
  @apply bg-[#26292E]/60 border border-[#35383E] rounded-xl p-4 text-center shadow;
}
.label {
  @apply text-sm text-gray-400 mb-1;
}
.value {
  @apply text-xl font-semibold;
}

.cat-row {
  @apply bg-[#26292E]/60 border border-[#35383E] p-4 rounded-xl flex items-center gap-4 shadow;
}
</style>
