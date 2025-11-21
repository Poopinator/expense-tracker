<template>
  <div class="ring-wrapper">
    <svg class="ring" viewBox="0 0 120 120">
      <circle
        class="ring-bg"
        cx="60"
        cy="60"
        r="54"
      />

      <circle
        class="ring-progress"
        cx="60"
        cy="60"
        r="54"
        :style="{
          strokeDasharray: circumference,
          strokeDashoffset: dashOffset
        }"
      />
    </svg>

    <div class="center-content">
      <slot>
        <p class="percent">{{ displayPercent }}%</p>
      </slot>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  percent: { type: Number, required: true }, // 0–100
  size: { type: Number, default: 120 },
  stroke: { type: Number, default: 10 }
});

const radius = 54;
const circumference = 2 * Math.PI * radius;

const normalized = computed(() =>
  Math.min(100, Math.max(0, props.percent))
);

const dashOffset = computed(() =>
  circumference - (normalized.value / 100) * circumference
);

const displayPercent = computed(() =>
  normalized.value.toFixed(0)
);
</script>

<style scoped>
.ring-wrapper {
  @apply relative w-[120px] h-[120px] flex items-center justify-center;
}

.ring {
  transform: rotate(-90deg);
}

.ring-bg {
  fill: none;
  stroke: rgba(255, 255, 255, 0.07);
  stroke-width: 10;
}

.ring-progress {
  fill: none;
  stroke: #818cf8;
  stroke-width: 10;
  stroke-linecap: round;
  transition: stroke-dashoffset 1s ease;
}

.center-content {
  @apply absolute inset-0 flex items-center justify-center;
}

.percent {
  @apply text-xl font-semibold text-white;
}
</style>


