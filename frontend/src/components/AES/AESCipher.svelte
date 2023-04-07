<script lang="ts">
  import axios from "axios";

  let key: string, iv: string, keyLength: number, file: File;

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

      const response = await axios
        .post(
          `http://localhost:5064/aes/encrypt?keyLength=${keyLength}`,
          formData,
          { responseType: "blob" }
        )
        .then((res) => {
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

  const handleKeySave = () => {
    const element = document.createElement("a");
    const file = new Blob([key], { type: "text/plain" });
    element.href = URL.createObjectURL(file);
    element.download = "key.txt";
    document.body.appendChild(element); // Required for this to work in FireFox
    element.click();
  };

  const handleIvSave = () => {
    const element = document.createElement("a");
    const file = new Blob([iv], { type: "text/plain" });
    element.href = URL.createObjectURL(file);
    element.download = "iv.txt";
    document.body.appendChild(element); // Required for this to work in FireFox
    element.click();
  };
</script>

<form on:submit|preventDefault={handleSubmit}>
  <h2>Šifriranje</h2>
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
      <br />
      <span>{key}</span>
      <button type="button" on:click={handleKeySave} class="saveBtn">
        Shrani
      </button>
    </div>
  {/if}
  <br />
  {#if iv}
    <div class="form-input">
      <label for="iv">IV</label>
      <br />
      <span>{iv}</span>
      <button type="button" on:click={handleIvSave} class="saveBtn">
        Shrani
      </button>
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
    padding: 15px;
    margin: 10px;
    border: 1px solid black;
    border-radius: 10px;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.05);
  }
  form > h2 {
    margin-top: 0px;
  }
  .form-input {
    margin-bottom: 15px;
  }
  .saveBtn {
    margin-left: 10px;
  }
</style>
