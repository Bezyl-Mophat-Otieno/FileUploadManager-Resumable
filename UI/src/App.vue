<script setup lang="ts">

const handleOnFileSelection = () => {};

</script>

<template>
  <div class="upload-container">
    <div class="upload-card">
      <h2 class="title">File Uplooad Manager</h2>

      <!-- File Input -->
      <label class="upload-box">
        <input
          type="file"
          class="hidden-input"
          @change="handleOnFileSelection"
        />
        <div class="upload-content">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="icon"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 16v2a2 2 0 002 2h12a2 2 0 002-2v-2M12 12V4m0 0L8 8m4-4l4 4"
            />
          </svg>
          <p class="upload-text">Click to browse or drag & drop a file</p>
        </div>
      </label>

      <!-- Progress Bar -->
      <div class="progress-container">
        <div class="progress-bar"></div>
      </div>

      <!-- Preview Button -->
      <button class="preview-btn" disabled>
        Preview File
      </button>
    </div>
  </div>
</template>

<style scoped>
.upload-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f8f9fb;
  padding: 1.5rem;
  font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif;
}

.upload-card {
  background: #fff;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);
  width: 100%;
  max-width: 420px;
  text-align: center;
}

.title {
  margin-bottom: 1.5rem;
  font-size: 1.4rem;
  font-weight: 600;
  color: #333;
}

.upload-box {
  display: block;
  position: relative;
  border: 2px dashed #cfd6e0;
  border-radius: 0.75rem;
  padding: 2.5rem 1rem;
  cursor: pointer;
  transition: border-color 0.25s ease;
}

.upload-box:hover {
  border-color: #6c63ff;
}

.hidden-input {
  display: none;
}

.upload-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}

.icon {
  width: 48px;
  height: 48px;
  color: #6c63ff;
}

.upload-text {
  color: #555;
  font-size: 0.95rem;
}

.progress-container {
  width: 100%;
  height: 8px;
  background: #e8ecf3;
  border-radius: 5px;
  overflow: hidden;
  margin-top: 1.5rem;
}

.progress-bar {
  width: 0%;
  height: 100%;
  background: linear-gradient(90deg, #6c63ff, #b29bff);
  transition: width 0.3s ease;
}

.preview-btn {
  margin-top: 1.5rem;
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 0.5rem;
  background: #6c63ff;
  color: #fff;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.3s ease, opacity 0.3s ease;
}

.preview-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.preview-btn:not(:disabled):hover {
  background: #5b55e4;
}
</style>
