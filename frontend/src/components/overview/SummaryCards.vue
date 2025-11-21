<template>
  <div class="grid md:grid-cols-3 gap-5 mb-8">

    <!-- Today -->
    <div class="card-graphite">
      <h3 class="label">🕒 Today</h3>
      <p class="value">${{ fmt(summary.today) }}</p>
    </div>

    <!-- This Month -->
    <div class="card-graphite">
      <h3 class="label">📅 This Month</h3>
      <p class="value">${{ fmt(summary.month) }}</p>
    </div>

    <!-- Total -->
    <div class="card-graphite">
      <h3 class="label">🌍 Total</h3>
      <p class="value">${{ fmt(summary.total) }}</p>
    </div>

  </div>
</template>

<script setup>
/* Prevent undefined errors by providing safe defaults */
const props = defineProps({
  summary: {
    type: Object,
    required: true,
    default: () => ({
      today: 0,
      month: 0,
      total: 0
    })
  }
});

/* Formatting helper */
const fmt = (n) => Number(n || 0).toFixed(2);
</script>

<style scoped>
.card-graphite {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl p-6 text-center shadow-md 
         hover:shadow-indigo-500/20 transition transform hover:-translate-y-1;
}

.label {
  @apply text-gray-400 text-sm mb-1;
}

.value {
  @apply text-3xl font-bold text-indigo-400;
}
</style>
