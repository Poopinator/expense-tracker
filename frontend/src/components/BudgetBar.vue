<template>
  <div class="w-full bg-[#1D1F22] h-2 rounded-full overflow-hidden">
    <div
      class="h-full transition-all duration-500"
      :class="percentage > 100 ? 'bg-red-500' : 'bg-indigo-500'"
      :style="{ width: Math.min(percentage, 100) + '%' }"
    />
  </div>
</template>

<script setup>
defineProps({
  percentage: { type: Number, default: 0 }
})
</script>
