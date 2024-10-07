<template>
    <div>
        <p>You should get redirected. <nuxt-link to="/">Click here if you are not redirected</nuxt-link></p>
    </div>
</template>
<script setup>
onMounted(() => {
    navigateTo('/');
})
</script>