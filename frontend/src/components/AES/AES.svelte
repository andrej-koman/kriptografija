<script lang="ts">
    import AesCipher from "./AESCipher.svelte";
    import AesDecipher from "./AESDecipher.svelte";
</script>

<div class="aes">
    <AesCipher />
    <AesDecipher />
</div>

<style>
    .aes {
        display: flex;
        flex-direction: row;
        justify-content: space-around;
    }
</style>