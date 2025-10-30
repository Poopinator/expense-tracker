<template>
  <canvas ref="canvas" class="particle-canvas"></canvas>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue"

const canvas = ref(null)
let ctx, particles = [], animationFrame
const COUNT = 80
const COLORS = ["#6366F1", "#60A5FA", "#A5B4FC"]

function initParticles() {
  const { width, height } = canvas.value
  particles = Array.from({ length: COUNT }, () => ({
    x: Math.random() * width,
    y: Math.random() * height,
    r: Math.random() * 1.8 + 0.6,
    dx: (Math.random() - 0.5) * 0.6,
    dy: (Math.random() - 0.5) * 0.6,
    color: COLORS[Math.floor(Math.random() * COLORS.length)],
    alpha: Math.random() * 0.5 + 0.5,
    pulse: Math.random() * 0.02 + 0.005,
  }))
}

function drawParticles() {
  const { width, height } = canvas.value

  // translucent background so trails remain
  ctx.fillStyle = "rgba(20,21,23,0.2)"
  ctx.fillRect(0, 0, width, height)

  for (const p of particles) {
    // twinkle
    p.alpha += p.pulse
    if (p.alpha > 1 || p.alpha < 0.3) p.pulse *= -1

    // draw particle
    ctx.beginPath()
    ctx.globalAlpha = p.alpha
    ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2)
    ctx.fillStyle = p.color
    ctx.shadowBlur = 8
    ctx.shadowColor = p.color
    ctx.fill()
    ctx.closePath()

    // connecting lines
    for (const q of particles) {
      const dist = Math.hypot(p.x - q.x, p.y - q.y)
      if (dist < 100) {
        const opacity = 0.05 * (1 - dist / 100)
        ctx.beginPath()
        ctx.moveTo(p.x, p.y)
        ctx.lineTo(q.x, q.y)
        ctx.strokeStyle = `rgba(99,102,241,${opacity})`
        ctx.lineWidth = 0.4
        ctx.stroke()
      }
    }

    // movement
    p.x += p.dx
    p.y += p.dy
    if (p.x < 0 || p.x > width) p.dx *= -1
    if (p.y < 0 || p.y > height) p.dy *= -1
  }

  animationFrame = requestAnimationFrame(drawParticles)
}

onMounted(() => {
  const c = canvas.value
  const scale = window.devicePixelRatio || 1
  c.width = window.innerWidth * scale
  c.height = window.innerHeight * scale
  ctx = c.getContext("2d")
  ctx.scale(scale, scale)

  initParticles()
  drawParticles()

  const resizeHandler = () => {
    c.width = window.innerWidth * scale
    c.height = window.innerHeight * scale
    ctx.scale(scale, scale)
    initParticles()
  }
  window.addEventListener("resize", resizeHandler)
  onBeforeUnmount(() => window.removeEventListener("resize", resizeHandler))
})

onBeforeUnmount(() => cancelAnimationFrame(animationFrame))
</script>

<style scoped>
.particle-canvas {
  position: fixed !important;
  inset: 0 !important;
  z-index: -10 !important;
  width: 100% !important;
  height: 100% !important;
  background: transparent !important;
  pointer-events: none !important;
  display: block !important;
}
</style>
