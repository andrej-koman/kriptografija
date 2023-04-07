<script lang="ts">
  import axios from "axios";

  let key: string, iv: string, keyLength: number;

  let file: File;

  function handleFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input) {
      file = input.files[0];
    }
  }

  async function handleSubmit() {
  if (!file || !keyLength) {
    alert("Niste vpisali vseh podatkov!");
    return;
  }
  try {
    const formData = new FormData();
    formData.append("file", file);

    const response = await axios.post(
      `http://localhost:5064/aes/encrypt?keyLength=${keyLength}`,
      formData,
      { responseType: "blob" }
    ).then(res => {
      key = res.headers["key"];
      iv = res.headers["iv"];
      return res;
    });

    const encryptedFile = new Blob([response.data], {
      type: "application/octet-stream",
    });
    const downloadLink = document.createElement("a");
    downloadLink.href = URL.createObjectURL(encryptedFile);
    downloadLink.download = `${file.name
      .split(".")
      .shift()}_encrypted.${file.name.split(".").pop()}`;
    downloadLink.click();
  } catch (error) {
    console.error(error);
  }
}

</script>

<form on:submit|preventDefault={handleSubmit}>
  <div class="form-input">
    <label for="file">Datoteka</label>
    <br />
    <input type="file" on:change={handleFileChange} name="file" />
  </div>
  <div class="form-input">
    <label for="keyLength">Velikost ključa</label>
    <br />
    <select name="keyLength" bind:value={keyLength}>
      <option value="128">128 bitov</option>
      <option value="192">192 bitov</option>
      <option value="256">256 bitov</option>
    </select>
  </div>
  <br />
  {#if key}
  <div class="form-input">
    <label for="key">Ključ</label>
    <input type="text" bind:value={key} name="key" readonly />
  </div>
  {/if}
  <br />
  {#if iv}
  <div class="form-input">
    <label for="iv">IV</label>
    <br />
    <input type="text" bind:value={iv} name="iv" readonly />
  </div>
  {/if}
  <br />
  <button type="submit">Šifriraj</button>
</form>

<style>
  form {
    display: flex;
    flex-direction: column;
    text-align: start;
  }
  .form-input {
    margin-bottom: 15px;
  }
</style>
