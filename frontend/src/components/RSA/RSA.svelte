<script lang="ts">
    import RsaCipher from "./RSACipher.svelte";
    import RsaDecipher from "./RSADecipher.svelte";

    import { createEventDispatcher } from "svelte";
    const dispatch = createEventDispatcher();

    const handleBackClick = () => {
        dispatch("back");
    }
</script>

<div class="aes">
    <RsaCipher />
    <RsaDecipher />
</div>
<button on:click={handleBackClick}>Nazaj</button>

<style>
    .aes {
        display: flex;
        flex-direction: row;
        justify-content: space-around;
    }
</style>