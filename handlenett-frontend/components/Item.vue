<template>
    <div class="groceryitem">
        <label :class="{ 'complete': props.isComplete }">
            <input type="checkbox" :checked="props.isComplete" @change="update"> {{ props.name }}
        </label>
        <button @click="deleteItem">X</button>
    </div>
</template>
<script setup>

const emit = defineEmits(['changed'])

const props = defineProps({
    id: { type: String },
    name: { type: String },
    isComplete: { type: Boolean, default: false }
})

const update = () => {
    emit('changed', { name: props.name, isComplete: !props.isComplete, id: props.id })
}
const deleteItem = () => {
    emit('delete', { id: props.id })
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

.complete {
    text-decoration: line-through;
    font-weight: 100;
}

input[type=checkbox] {
    visibility: hidden;
}
</style>