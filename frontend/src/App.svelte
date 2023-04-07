<script lang="ts">
  import Aes from "./components/AES/AES.svelte";
  import Rsa from "./components/RSA/RSA.svelte";

  let currentComponent = null;
</script>

{#if currentComponent === null}
<main>
  <div>
    <h2>Šifriranje</h2>
    <button on:click={() => currentComponent = Aes}>AES</button>
    <button on:click={() => currentComponent = Rsa}>RSA</button>
  </div>
</main>
{:else if currentComponent === Aes}
  <Aes />
{:else if currentComponent === Rsa}
  <Rsa />
{/if}


<style>
</style>
