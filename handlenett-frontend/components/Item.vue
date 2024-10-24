<template>
    <div class="groceryitem">
        <div>
            <label :class="{ 'complete': status }">
                <input type="checkbox" :checked="status" @change="update"> {{ props.element?.name }}
            </label>
            <span @click="lmfgi">❔</span>
            <div class="gi-creator">📝{{ createdBy }}</div>
        </div>
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

const lmfgi = () => {
    window.open(`https://www.google.com/search?q=bunnpris ${props.element.name}&udm=2`, '_blank')
}

const update = () => {
    status.value = !status.value;
    emit('changed', { id: props.element.id, name: props.element.name, isCompleted: status.value })
}
const deleteeleement = () => {
    emit('delete', { id: props.element.id })
}

const createdBy = computed(() => {

    const fullEmail = props.element.createdBy;
    const dottedName = fullEmail.split('@')[0];
    const firstName = dottedName.split('.')[0];
    return firstName.charAt(0).toUpperCase() + firstName.slice(1);
});

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

.gi-creator {
    font-size: 0.8rem;
    font-weight: 100;
    padding-left: 1rem;
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