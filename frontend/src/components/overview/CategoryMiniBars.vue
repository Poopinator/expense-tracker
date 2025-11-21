<template>
  <div class="panel">
    <h3 class="panel-title">Top Categories (This Period)</h3>

    <div v-if="sorted.length === 0" class="empty">
      No category data available.
    </div>

    <ul v-else class="space-y-4">
      <li
        v-for="c in sorted"
        :key="c.category"
        class="cat-item"
      >
        <div class="flex justify-between text-sm mb-1">
          <span class="text-gray-300 font-medium">{{ c.category }}</span>
          <span class="text-indigo-400 font-semibold">${{ c.amount.toFixed(2) }}</span>
        </div>

        <div class="bar-bg">
          <div
            class="bar-fill"
            :style="{ width: (c.amount / topAmount * 100) + '%' }"
          ></div>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  categories: {
    type: Array,
    default: () => [] // [{ category, amount }]
  }
});

// Sort by highest amounts
const sorted = computed(() => {
  return [...props.categories]
    .sort((a, b) => b.amount - a.amount)
    .slice(0, 5);
});

const topAmount = computed(() =>
  sorted.value.length ? sorted.value[0].amount : 1
);
</script>

<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E]
         rounded-2xl shadow-lg p-6 backdrop-blur-xl;
}

.panel-title {
  @apply text-white font-semibold text-lg mb-4;
}

.empty {
  @apply text-gray-500 text-center py-4;
}

.cat-item {
  @apply p-2 bg-[#26292E]/60 rounded-xl border border-[#35383E]
         hover:border-indigo-500 transition;
}

.bar-bg {
  @apply w-full h-2 bg-gray-700/40 rounded-full overflow-hidden;
}

.bar-fill {
  @apply h-full bg-indigo-500 rounded-full transition-all;
}
</style>


