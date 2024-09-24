<template>
  <h1>Handlenett</h1>
  <div>
    <div>
      <Item v-for="i in items" :name="i.name" :isComplete="i.isComplete" :id="i.id" :key="i.id"
        @changed="updatedItem" />
    </div>
    <NewItem @changed="newItem"></NewItem>

    <!-- <pre>
      {{ items }}
    </pre> -->
  </div>
</template>
<script setup>
import { ref } from 'vue'
const items = ref([{ id: "1", name: 'Juice', isComplete: false }, { id: "2", name: 'Jubelsalami', isComplete: true }])

const updatedItem = (updatedItem) => {
  console.info('updatedItem... > ', updatedItem)
  let i = items.value.find(i => i.id === updatedItem.id)
  const idx = items.value.indexOf(i)
  items.value[idx] = updatedItem;
}

const newItem = (newItem) => {
  if (newItem.name === '') return
  items.value.push({ name: newItem.name, isComplete: newItem.isComplete, id: Math.random().toString(36).substr(2, 9) })
}

</script>
