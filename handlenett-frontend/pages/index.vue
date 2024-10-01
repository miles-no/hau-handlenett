<template>
    <div v-if="loading" class="loadData">
        <div class="loader"></div>
    </div>

    <h1>Handlenett</h1>
    <div>
        <div style="margin-bottom: 2rem;">
            <Item v-for="i in items" :name="i.name" :isComplete="i.isComplete" :id="i.id" :key="i.id"
                @changed="updatedItem" @delete="deleteItem" />
        </div>
        <NewItem @changed="newItem"></NewItem>
    </div>

</template>
<script setup>
import { ref } from 'vue'
const items = ref([])

const loading = ref(true)

onMounted(async () => {

    loading.value = true
    useHttp('Item', 'GET').then(data => {
        items.value = data;
    });
    loading.value = false
})

const updatedItem = (updatedItem) => {
    useHttp(`Item/${updatedItem.id}`, 'PUT', { name: updatedItem.name, isComplete: updatedItem.isComplete }).then(data => {
        let i = items.value.find(i => i.id === updatedItem.id)
        const idx = items.value.indexOf(i)
        items.value[idx] = updatedItem;
    });
}

const newItem = (newItem) => {
    if (newItem.name === '') return

    useHttp('Item', 'POST', { name: newItem.name }).then(data => {
        items.value.push({ name: data.name, isComplete: data.isComplete, id: data.id })
    });

}

const deleteItem = (deleteItem) => {
    useHttp(`Item/${deleteItem.id}`, 'DELETE').then(data => {
        let i = items.value.find(i => i.id === deleteItem.id)
        const idx = items.value.indexOf(i)
        items.value.splice(idx, 1)
    });
}

</script>
