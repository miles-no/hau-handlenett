<template>
    <div class="groceryitem">
        <label :class="{ 'complete': status }">
            <input type="checkbox" :checked="status" @change="update"> {{ props.element?.name }}
        </label>
        <div class="button-group">
            <button v-if="false" class="btn primary" @click="editeleement">✏️
            </button>
            <button v-if="isElementDeletable" class="btn warning" @click="deleteeleement">🗑️
            </button>
        </div>
    </div>
</template>
<script setup>
import { ref } from 'vue';
const emit = defineEmits(['changed'])

const status = ref(false)

const props = defineProps({
    element: { type: Object },
    isElementDeletable: { type: Boolean, default: false },
    isElementEditable: { type: Boolean, default: false },
})

onMounted(() => {
    status.value = props.element.isCompleted
})

const update = () => {
    status.value = !status.value;
    emit('changed', { id: props.element.id, name: props.element.name, isCompleted: status.value })
}
const deleteeleement = () => {
    emit('delete', { id: props.element.id })
}

</script>
<style scoped>
.groceryitem {
    display: flex;
    justify-content: space-between;
    padding: 1rem;
    border-bottom: 1px solid #ccc;
    font-size: 1.5rem;
    font-weight: bold;
}

.button-group {
    display: flex;
    gap: 1rem;
}

.complete {
    text-decoration: line-through;
    font-weight: 100;
}

input[type=checkbox] {
    visibility: hidden;
}
</style>