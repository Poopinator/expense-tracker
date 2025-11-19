<template>
  <div
    class="relative min-h-screen bg-gradient-to-b from-[#08090b] via-[#101118] to-[#050509] text-gray-100"
  >
    <!-- 🌌 Background Particles -->
    <div class="particle-bg"></div>

    <main
      class="relative max-w-6xl mx-auto px-6 pt-24 pb-20 flex flex-col items-center text-center"
    >
      <!-- ⭐ Continuous Infinite Scrolling Banner with Glow -->
      <div class="banner-shell mb-14">
        <div class="banner-glow"></div>
        <div class="banner-track">
          <!-- First lane -->
          <div class="banner-lane">
            <span
              v-for="(w, i) in bannerWords"
              :key="'lane1-' + i"
              class="banner-item"
            >
              {{ w }}
            </span>
          </div>
          <!-- Second lane -->
          <div class="banner-lane">
            <span
              v-for="(w, i) in bannerWords"
              :key="'lane2-' + i"
              class="banner-item"
            >
              {{ w }}
            </span>
          </div>
        </div>
      </div>

      <!-- 💸 Hero Section -->
      <section class="w-full flex flex-col items-center">
        <h1 class="text-4xl md:text-5xl lg:text-6xl font-extrabold mb-4 tracking-tight">
          Take Control of Your Finances 💰
        </h1>

        <!-- ✨ Rotating subtitle -->
        <h2
          class="text-lg md:text-2xl text-indigo-400 font-semibold h-8 overflow-hidden mb-6"
        >
          <transition-group name="fade-up" tag="div">
            <div :key="currentWord" class="animated-word">
              {{ currentWord }}
            </div>
          </transition-group>
        </h2>

        <p class="text-gray-400 max-w-xl mb-10 text-sm md:text-base">
          Track expenses effortlessly, set budgets with precision, and visualize your
          spending habits with clear, modern insights designed for real life.
        </p>

        <!-- 🚀 CTAs -->
        <div class="flex flex-wrap justify-center gap-4 mb-14">
          <RouterLink to="/register" class="btn-primary">
            Get Started — It’s Free
          </RouterLink>

          <RouterLink to="/login" class="btn-secondary">
            Already have an account? Log In
          </RouterLink>
        </div>
      </section>

      <!-- 💡 Feature Highlights -->
      <section class="grid md:grid-cols-3 gap-6 w-full mb-14">
        <div class="feature-card">
          <h3 class="feature-title">📊 Visual Insights</h3>
          <p class="feature-text">
            Watch your spending patterns come to life with elegant, interactive charts.
          </p>
        </div>

        <div class="feature-card">
          <h3 class="feature-title">🎯 Smart Budgets</h3>
          <p class="feature-text">
            Set realistic limits by category and stay ahead with gentle, timely nudges.
          </p>
        </div>

        <div class="feature-card">
          <h3 class="feature-title">🔒 Secure by Design</h3>
          <p class="feature-text">
            Your financial data is protected with secure authentication and best practices.
          </p>
        </div>
      </section>

      <!-- 🔻 Bottom Section: Why + Quick Stats -->
      <section
        class="w-full grid md:grid-cols-[1.1fr,0.9fr] gap-8 items-stretch text-left"
      >
        <!-- Why ExpenseMate -->
        <div
          class="rounded-2xl bg-gradient-to-br from-[#151729] via-[#10121f] to-[#05060b] border border-[#27293a] shadow-[0_18px_45px_rgba(15,23,42,0.7)] p-6 md:p-8"
        >
          <h3 class="text-xl md:text-2xl font-semibold mb-3 text-white">
            Built for everyday money decisions.
          </h3>
          <p class="text-gray-400 text-sm md:text-base mb-5 leading-relaxed">
            ExpenseMate gives you a live view of where your money goes so you can cut
            wasteful spending, strengthen your savings, and feel confident about every
            purchase — without wrestling with spreadsheets.
          </p>

          <ul class="space-y-3 text-sm md:text-base text-gray-300">
            <li class="flex items-start gap-2">
              <span class="mt-1 text-indigo-400">•</span>
              <span>Log expenses in seconds from any device.</span>
            </li>
            <li class="flex items-start gap-2">
              <span class="mt-1 text-indigo-400">•</span>
              <span>See category-level breakdowns for instant clarity.</span>
            </li>
            <li class="flex items-start gap-2">
              <span class="mt-1 text-indigo-400">•</span>
              <span>Stay aligned with your goals using budget alerts.</span>
            </li>
          </ul>
        </div>

        <!-- Quick Stats / Highlights -->
        <div
          class="rounded-2xl bg-[#10111b]/90 border border-[#27293a] backdrop-blur-xl p-6 md:p-7 flex flex-col justify-between"
        >
          <h4 class="text-sm font-semibold text-indigo-300 uppercase tracking-[0.18em] mb-4">
            At a Glance
          </h4>

          <div class="space-y-4 mb-4">
            <div class="stat-chip">
              <span class="stat-label">Add an expense in</span>
              <span class="stat-value">~3 seconds</span>
            </div>
            <div class="stat-chip">
              <span class="stat-label">Track up to</span>
              <span class="stat-value">∞ categories</span>
            </div>
            <div class="stat-chip">
              <span class="stat-label">See trends over</span>
              <span class="stat-value">days, months, years</span>
            </div>
          </div>

          <p class="text-xs text-gray-500">
            No ads. No noise. Just a focused space to understand and improve your spending.
          </p>
        </div>
      </section>

      <!-- 📎 Footer CTA -->
      <p class="mt-14 text-gray-500 text-xs md:text-sm text-center">
        Made by <span class="text-gray-300">ExpenseMate</span> — Your personal
        finance companion.
      </p>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";

// Rotating subtitle under main heading
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

onMounted(() => {
  intervalId = setInterval(() => {
    wordIndex = (wordIndex + 1) % rotatingWords.length;
    currentWord.value = rotatingWords[wordIndex];
  }, 2500);
});

onBeforeUnmount(() => {
  if (intervalId) clearInterval(intervalId);
});

// Banner words – focused on expense tracking vibes
const bannerWords = [
  "Track Smarter",
  "Budget with Confidence",
  "Visualize Spending",
  "Stay Organized",
  "Save More Easily",
  "Understand Your Money",
];

</script>

<style scoped>
/* 💫 Slim, smooth feature cards */
.feature-card {
  padding: 1.4rem 1.6rem;
  border-radius: 1.15rem;
  background: radial-gradient(circle at 0% -20%, rgba(129, 140, 248, 0.16), transparent 55%),
    rgba(17, 24, 39, 0.9);
  border: 1px solid rgba(55, 65, 81, 0.7);
  backdrop-filter: blur(10px);
  transition: transform 0.2s ease, box-shadow 0.2s ease, border-color 0.2s ease;
}
.feature-card:hover {
  transform: translateY(-4px);
  border-color: rgba(129, 140, 248, 0.9);
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.85);
}
.feature-title {
  color: #a5b4fc;
  font-size: 1.05rem;
  font-weight: 600;
  margin-bottom: 0.45rem;
}
.feature-text {
  font-size: 0.9rem;
  color: #d1d5db;
}

/* 🎛 Buttons */
.btn-primary {
  background: radial-gradient(circle at 10% 0%, #818cf8, #4f46e5);
  padding: 0.8rem 1.8rem;
  border-radius: 9999px;
  font-weight: 600;
  color: white;
  font-size: 0.95rem;
  box-shadow: 0 18px 40px rgba(79, 70, 229, 0.6);
  transition: transform 0.18s ease, box-shadow 0.18s ease, filter 0.18s ease;
}
.btn-primary:hover {
  transform: translateY(-1px) scale(1.01);
  filter: brightness(1.05);
  box-shadow: 0 20px 45px rgba(79, 70, 229, 0.75);
}
.btn-secondary {
  border: 1px solid #4b5563;
  padding: 0.8rem 1.8rem;
  border-radius: 9999px;
  color: #e5e7eb;
  font-weight: 500;
  font-size: 0.95rem;
  background: rgba(17, 24, 39, 0.9);
  transition: border-color 0.18s ease, color 0.18s ease, background 0.18s ease,
    transform 0.18s ease;
}
.btn-secondary:hover {
  border-color: #818cf8;
  color: #ffffff;
  background: rgba(17, 24, 39, 1);
  transform: translateY(-1px);
}

/* 🔁 Rotating subtitle animation */
.fade-up-enter-active,
.fade-up-leave-active {
  transition: all 0.45s ease;
}
.fade-up-enter-from,
.fade-up-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
.animated-word {
  height: 32px; /* keeps layout from jumping */
}

/* 🧷 Clean, premium marquee banner — NO lag, NO clutter */
.banner-shell {
  width: 100%;
  overflow: hidden;
  padding-block: 0.3rem; /* slimmer banner */
  margin-bottom: 2.5rem; /* more breathing room */
  opacity: 0.75; /* cleaner, less heavy */
}

.banner-track {
  display: flex;
  width: 180%;
  animation: banner-slide 26s linear infinite; /* slower = less laggy */
}

.banner-lane {
  flex: 0 0 50%;
  display: flex;
  justify-content: space-evenly;
  align-items: center;
  gap: 3rem;
}

.banner-item {
  font-size: 0.9rem;
  color: #d2d6f5;
  white-space: nowrap;
  letter-spacing: 0.3px;
}

/* subtle, premium glow */
.banner-glow {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  width: 45%;
  height: 1.2rem;
  top: 60%;
  background: radial-gradient(circle, rgba(129,140,248,0.18), transparent 70%);
  filter: blur(20px);
  opacity: 0.45;
  z-index: -1;
}

@keyframes banner-slide {
  0% { transform: translateX(0); }
  100% { transform: translateX(-50%); }
}



/* Quick stats chips */
.stat-chip {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.7rem 0.85rem;
  border-radius: 0.9rem;
  background: radial-gradient(circle at 0% 0%, rgba(79, 70, 229, 0.18), transparent 60%),
    rgba(15, 23, 42, 0.9);
  border: 1px solid rgba(55, 65, 81, 0.8);
}
.stat-label {
  font-size: 0.8rem;
  color: #9ca3af;
}
.stat-value {
  font-size: 0.85rem;
  font-weight: 600;
  color: #e5e7eb;
}

/* 🌌 Particle background */
.particle-bg {
  position: absolute;
  inset: 0;
  z-index: -10;
  pointer-events: none;
  background:
    radial-gradient(circle at 15% 20%, rgba(129, 140, 248, 0.25), transparent 55%),
    radial-gradient(circle at 80% 75%, rgba(236, 72, 153, 0.2), transparent 55%),
    radial-gradient(circle at 50% 50%, rgba(59, 130, 246, 0.14), transparent 65%);
  animation: drift 24s infinite alternate ease-in-out;
}
@keyframes drift {
  0% {
    background-position: 20% 25%, 80% 75%, 50% 50%;
  }
  100% {
    background-position: 65% 35%, 25% 70%, 55% 55%;
  }
}
</style>
