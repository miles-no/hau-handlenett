<template>
    <div v-if="loading" class="loadData">
        <div class="loader"></div>
    </div>
    <h1>Handlenett</h1>
    <div>
        <div style="margin-bottom: 2rem;">
            <Item v-for="i in items" :key="i.id" @changed="updatedItem" @delete="deleteItem"
                :isElementDeletable="currentUser === i.createdBy" :element="i" />
        </div>
        <NewItem @changed="newItem"></NewItem>
    </div>
    <!-- <pre>
        {{ items }}
     </pre> -->
</template>
<script setup>
import { ref } from 'vue'
const { $getAccounts } = useNuxtApp();

const items = ref([])
const loading = ref(true)
const currentUser = ref("")

onMounted(async () => {

    loading.value = true
    useHttp('Item', 'GET').then(data => {
        items.value = data;
    });
    loading.value = false

    currentUser.value = await $getAccounts()[0].username;

})

const updatedItem = (updatedItem) => {
    console.info('updatedItem', updatedItem)
    useHttp(`Item/${updatedItem.id}`, 'PUT', { name: updatedItem.name, isCompleted: updatedItem.isCompleted }).then(data => {
        let i = items.value.find(i => i.id === updatedItem.id)
        const idx = items.value.indexOf(i)
        console.log('updated', data)
        items.value[idx] = data;
    });
}

const newItem = (newItem) => {
    if (newItem.name === '') return

    useHttp('Item', 'POST', { name: newItem.name }).then(data => {
        items.value.push(data)
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
