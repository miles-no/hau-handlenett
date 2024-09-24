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
    const objCopy = Object.assign({}, { name: item.value, isComplete: false })
    emit('changed', objCopy)
    item.value = ''
}
</script>
<style scoped></style>