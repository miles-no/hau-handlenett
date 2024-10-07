<template>
    <h1>Handlenett</h1>
    <div v-if="loading" class="loadData">
        <div class="loader"></div>
    </div>
    <div v-else>
        <div style="margin-bottom: 2rem;">
            <Item v-for="i in items" :key="i.id" @changed="updatedItem" @delete="deleteItem"
                :isElementDeletable="currentUser === i.createdBy" :isElementEditable="currentUser === i.createdBy"
                :element="i" />
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
        loading.value = false

        const sortArray = (arr) => {
            return arr.sort((a, b) => {
                return a.name.localeCompare(b.name);
            });
        }

        const isNotCompleted = data.filter(i => !i.isCompleted)
        const isCompleted = data.filter(i => i.isCompleted)

        items.value = [...sortArray(isNotCompleted), ...sortArray(isCompleted)];

    });

    currentUser.value = await $getAccounts()[0].username;

})

const updatedItem = (updatedItem) => {
    useHttp(`Item/${updatedItem.id}`, 'PUT', { name: updatedItem.name, isCompleted: updatedItem.isCompleted }).then(data => {
        let i = items.value.find(i => i.id === updatedItem.id)
        const idx = items.value.indexOf(i)
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

<style scoped>
/* HTML: <div class="loader"></div> */
.loader {
    width: 80px;
    height: 40px;
    border-radius: 0 0 100px 100px;
    border: 5px solid #538a2d;
    border-top: 0;
    box-sizing: border-box;
    background:
        radial-gradient(farthest-side at top, #0000 calc(100% - 5px), #e7ef9d calc(100% - 4px)),
        radial-gradient(2px 3px, #5c4037 89%, #0000) 0 0/17px 12px,
        #ff1643;
    --c: radial-gradient(farthest-side, #000 94%, #0000);
    -webkit-mask:
        linear-gradient(#0000 0 0),
        var(--c) 12px -8px,
        var(--c) 29px -8px,
        var(--c) 45px -6px,
        var(--c) 22px -2px,
        var(--c) 34px 6px,
        var(--c) 21px 6px,
        linear-gradient(#000 0 0);
    mask:
        linear-gradient(#000 0 0),
        var(--c) 12px -8px,
        var(--c) 29px -8px,
        var(--c) 45px -6px,
        var(--c) 22px -2px,
        var(--c) 34px 6px,
        var(--c) 21px 6px,
        linear-gradient(#0000 0 0);
    -webkit-mask-composite: destination-out;
    mask-composite: exclude, add, add, add, add, add, add;
    -webkit-mask-repeat: no-repeat;
    animation: l8 3s infinite;
}

@keyframes l8 {
    0% {
        -webkit-mask-size: auto, 0 0, 0 0, 0 0, 0 0, 0 0, 0 0
    }

    15% {
        -webkit-mask-size: auto, 20px 20px, 0 0, 0 0, 0 0, 0 0, 0 0
    }

    30% {
        -webkit-mask-size: auto, 20px 20px, 20px 20px, 0 0, 0 0, 0 0, 0 0
    }

    45% {
        -webkit-mask-size: auto, 20px 20px, 20px 20px, 20px 20px, 0 0, 0 0, 0 0
    }

    60% {
        -webkit-mask-size: auto, 20px 20px, 20px 20px, 20px 20px, 20px 20px, 0 0, 0 0
    }

    75% {
        -webkit-mask-size: auto, 20px 20px, 20px 20px, 20px 20px, 20px 20px, 20px 20px, 0 0
    }

    90%,
    100% {
        -webkit-mask-size: auto, 20px 20px, 20px 20px, 20px 20px, 20px 20px, 20px 20px, 20px 20px
    }
}
</style>