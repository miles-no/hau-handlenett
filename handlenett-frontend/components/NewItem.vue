<template>
    <form @submit.prevent="update">
        <input type="text" placeholder="Trenger vi noe mer?" @keyup="typing" v-model="item">
        <button class="btn" @click="update">🛒</button>
    </form>
</template>
<script setup>
import { ref } from 'vue'
const item = ref('')
const emit = defineEmits(['changed', 'typing'])

const update = () => {
    emit('changed', { name: item.value, isComplete: false })
    item.value = ''
}

const typing = () => {
    if (item.value.length !== 1) {
        emit('typing', item.value)
    }
}   
</script>
<style scoped>
form {
    padding: 1rem;
    font-size: 1.5rem;
}
</style>