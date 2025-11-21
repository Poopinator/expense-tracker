<template>
  <div class="panel">
    <h2 class="panel-title">Insights</h2>

    <!-- Empty -->
    <p v-if="!safeInsights.length" class="empty-text">
      No insights available for this range.
    </p>

    <!-- List -->
    <ul v-else class="space-y-3">
      <li v-for="(i, idx) in safeInsights" :key="idx" class="insight-item">
        {{ i }}
      </li>
    </ul>
  </div>
</template>

<script setup>
import { computed } from "vue";   // ✅ YOU WERE MISSING THIS

const props = defineProps({
  insights: {
    type: Array,
    default: () => []   // prevents undefined
  }
});

// computed wrapper — always returns an array
const safeInsights = computed(() =>
  Array.isArray(props.insights) ? props.insights : []
);
</script>

<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl p-6 shadow-lg;
}
.panel-title {
  @apply text-xl font-semibold text-white mb-5;
}
.empty-text {
  @apply text-gray-600 text-center;
}
.insight-item {
  @apply bg-[#26292E] border border-[#35383E] p-3 rounded-lg text-gray-200 
         shadow hover:bg-[#2d3035] transition;
}
</style>
