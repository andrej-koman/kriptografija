<script lang="ts">
    import AesCipher from "./AESCipher.svelte";
    import AesDecipher from "./AESDecipher.svelte";

    import { createEventDispatcher } from "svelte";
    const dispatch = createEventDispatcher();

    const handleBackClick = () => {
        dispatch("back");
    }
</script>

<div class="aes">
    <AesCipher />
    <AesDecipher />
</div>
<button on:click={handleBackClick}>Nazaj</button>

<style>
    .aes {
        display: flex;
        flex-direction: row;
        justify-content: space-around;
    }
</style>