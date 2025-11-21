<template>
  <div class="panel">
    <h2 class="panel-title">Weekly Trend</h2>

    <div class="h-40 flex items-end gap-2">
      <!-- Bars -->
      <div
        v-for="(v, idx) in scaledValues"
        :key="idx"
        class="flex-1 flex flex-col items-center"
      >
        <div
          class="w-full rounded-t-md bg-indigo-500/70"
          :style="{ height: v + 'px' }"
        ></div>
        <span class="text-[10px] text-gray-400 mt-1">{{ labels[idx] }}</span>
      </div>
    </div>

    <p class="text-gray-400 text-xs mt-3 text-center">
      Total this week: <span class="text-indigo-300">${{ fmt(total) }}</span>
    </p>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  expenses: {
    type: Array,
    default: () => []     // <-- prevents undefined
  },
});

// Always guarantee an array
const exp = computed(() =>
  Array.isArray(props.expenses) ? props.expenses : []
);

// Build last 7 days
const days = Array.from({ length: 7 }, (_, i) => {
  const d = new Date();
  d.setDate(d.getDate() - (6 - i));
  d.setHours(0, 0, 0, 0);
  return d;
});

// Daily totals
const dailyTotals = computed(() =>
  days.map((day) =>
    exp.value
      .filter((e) => {
        if (!e || !e.date) return false;
        return new Date(e.date).toDateString() === day.toDateString();
      })
      .reduce((sum, e) => sum + Number(e.amount || 0), 0)
  )
);

// Scale bars
const scaledValues = computed(() => {
  const max = Math.max(...dailyTotals.value, 1);
  return dailyTotals.value.map((v) => (v / max) * 110);
});

// Labels (Mon–Sun)
const labels = computed(() =>
  days.map((d) => d.toLocaleDateString(undefined, { weekday: "short" }))
);

// Weekly total
const total = computed(() =>
  dailyTotals.value.reduce((s, v) => s + v, 0)
);

const fmt = (n) => Number(n).toFixed(2);
</script>


<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-5;
}

.panel-title {
  @apply text-white font-semibold mb-4;
}
</style>
