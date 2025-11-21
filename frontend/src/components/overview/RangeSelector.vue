<template>
  <div class="panel mb-6">
    <h2 class="panel-title">Select Date Range</h2>

    <div class="flex flex-wrap items-center gap-3">
      <!-- Preset Buttons -->
      <button
        v-for="p in presets"
        :key="p.label"
        @click="applyPreset(p)"
        :class="presetClass(p.label)"
      >
        {{ p.label }}
      </button>

      <!-- Manual Date Inputs -->
      <input type="date" v-model="start" class="select-graphite" @change="emitRange" />
      <input type="date" v-model="end" class="select-graphite" @change="emitRange" />
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  startDate: String,
  endDate: String,
});

const emit = defineEmits(["update:range"]);

// Local state
const start = ref(props.startDate);
const end = ref(props.endDate);

// Presets
const presets = [
  { label: "Today", type: "today" },
  { label: "7 Days", type: "7d" },
  { label: "30 Days", type: "30d" },
  { label: "This Month", type: "month" },
  { label: "YTD", type: "ytd" },
];

// Emit updated ranges
function emitRange() {
  emit("update:range", { start: start.value, end: end.value });
}

// Apply preset ranges
function applyPreset(p) {
  const now = new Date();

  if (p.type === "today") {
    start.value = now.toISOString().slice(0, 10);
    end.value = start.value;
  } else if (p.type === "7d") {
    const s = new Date(now);
    s.setDate(s.getDate() - 6);
    start.value = s.toISOString().slice(0, 10);
    end.value = now.toISOString().slice(0, 10);
  } else if (p.type === "30d") {
    const s = new Date(now);
    s.setDate(s.getDate() - 29);
    start.value = s.toISOString().slice(0, 10);
    end.value = now.toISOString().slice(0, 10);
  } else if (p.type === "month") {
    start.value = new Date(now.getFullYear(), now.getMonth(), 1)
      .toISOString()
      .slice(0, 10);
    end.value = now.toISOString().slice(0, 10);
  } else if (p.type === "ytd") {
    start.value = new Date(now.getFullYear(), 0, 1).toISOString().slice(0, 10);
    end.value = now.toISOString().slice(0, 10);
  }

  emitRange();
}

// Styling logic
function presetClass(label) {
  return [
    "px-3 py-2 rounded-lg border transition text-sm",
    "bg-[#1D1F22] border-[#35383E] text-gray-300 hover:text-white hover:border-indigo-500",
  ].join(" ");
}

// watch for initial prop updates
watch(
  () => [props.startDate, props.endDate],
  ([s, e]) => {
    start.value = s;
    end.value = e;
  }
);
</script>

<style scoped>
.panel {
  @apply bg-[#1D1F22]/80 border border-[#35383E] rounded-2xl backdrop-blur-xl shadow-lg p-5;
}

.panel-title {
  @apply text-white font-semibold mb-4;
}

.select-graphite {
  @apply px-3 py-2 rounded-lg bg-[#26292E] text-gray-200 border border-[#35383E]
         focus:ring-2 focus:ring-indigo-500 focus:outline-none;
}
</style>
