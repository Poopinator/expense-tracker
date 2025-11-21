<template>
  <div class="panel">
    <h2 class="panel-title">Category Breakdown</h2>

    <div v-if="sorted.length" class="space-y-4">
      <div
        v-for="c in sorted"
        :key="c.name"
        class="flex flex-col"
      >
        <div class="flex justify-between items-center mb-1">
          <span class="text-gray-300 text-sm">{{ c.name }}</span>
          <span class="text-indigo-300 font-semibold text-sm">${{ fmt(c.total) }}</span>
        </div>

        <!-- Progress Bar -->
        <div class="w-full bg-[#2a2d31] h-2 rounded-full overflow-hidden">
          <div
            class="h-full bg-indigo-500 rounded-full transition-all"
            :style="{ width: c.percent + '%' }"
          ></div>
        </div>
      </div>
    </div>

    <p v-else class="text-gray-500 text-sm mt-3 text-center">
      No category data found for this period.
    </p>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  expenses: {
    type: Array,
    default: () => []   // <-- prevents undefined
  },
});

// Always ensure an array
const exp = computed(() =>
  Array.isArray(props.expenses) ? props.expenses : []
);

// Group totals
const categoryTotals = computed(() => {
  const t = {};
  for (const e of exp.value) {
    if (!e || !e.category) continue;
    const amt = Number(e.amount || 0);
    t[e.category] = (t[e.category] || 0) + amt;
  }
  return t;
});

// Sorted top categories (max 5)
const sorted = computed(() => {
  const entries = Object.entries(categoryTotals.value).map(([name, total]) => ({
    name,
    total,
  }));

  const totalAll = entries.reduce((s, x) => s + x.total, 0) || 1;

  return entries
    .sort((a, b) => b.total - a.total)
    .slice(0, 5)
    .map((x) => ({
      ...x,
      percent: (x.total / totalAll) * 100,
    }));
});

const fmt = (n) => Number(n).toFixed(2);
</script>


<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E]
         rounded-2xl backdrop-blur-xl shadow-lg p-5;
}

.panel-title {
  @apply text-white font-semibold mb-4;
}
</style>
