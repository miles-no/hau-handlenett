<template>
  <h1>Handlenett</h1>
  <div>
    <div style="margin-bottom: 2rem;">
      <Item v-for="i in items" :name="i.name" :isComplete="i.isComplete" :id="i.id" :key="i.id"
        @changed="updatedItem" />
    </div>
    <NewItem @changed="newItem"></NewItem>

    <!-- <pre>
      {{ items }}
    </pre> -->
    <NuxtPage />
  </div>
</template>
<script setup>
import { ref } from 'vue'
const items = ref([])

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

onMounted(async () => {
  useHttp('Item', 'GET').then(data => {
    items.value = data;
  });
})

</script>
