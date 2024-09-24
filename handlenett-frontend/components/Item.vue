<template>
    <div>
        <label :class="{ 'complete': props.isComplete }">
            <input type="checkbox" :checked="props.isComplete" @change="update"> {{ props.name }}
        </label>
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

</script>
<style scoped>
input[type="checkbox"] {}

.complete {
    text-decoration: line-through;
}
</style>