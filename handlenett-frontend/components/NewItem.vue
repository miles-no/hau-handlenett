<template>
    <form @submit.prevent="update">
        <input type="text" placeholder="Hva mer?" v-model="item">
        <button @click="update">+</button>
    </form>
</template>
<script setup>
import { ref, defineEmits } from 'vue'
const item = ref('')
const emit = defineEmits(['changed'])

const update = () => {
    emit('changed', { name: item.value, isComplete: false })
    item.value = ''
}
</script>
<style scoped>
form {
    padding: 1rem;
    font-size: 1.5rem;
}
</style>