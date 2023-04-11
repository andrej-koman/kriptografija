<script lang="ts">
  import axios from "axios";

  let key: string, iv: string, file: File;

  const handleFileChange = (event: Event) => {
    const input = event.target as HTMLInputElement;
    if (input) {
      file = input.files[0];
    }
  };

  const handleSubmit = async () => {
    if (!file  || !key || !iv) {
      alert("Niste vpisali vseh podatkov!");
      return;
    }
    try {
      const formData = new FormData();
      formData.append("file", file);
      formData.append("key", key);
      formData.append("iv", iv);

      const response = await axios.post(
        `http://localhost:5064/aes/decrypt`,
        formData,
        {
          responseType: "blob",
        }
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");

      link.href = url;
      link.setAttribute("download", `${file.name.split("_").shift()}.${file.name.split(".").pop()}`);
      document.body.appendChild(link);
      link.click();
    } catch (error) {
      alert("Ključ je nepravilen!");
    }
  };

  const handleKeyImport = async () => {
    importFile("key");
  };

  const handleIvImport = async () => {
    importFile("iv");
  };

  const importFile = (type: "key" | "iv") => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".txt";
    input.onchange = (event) => {
      const file = (event.target as HTMLInputElement).files[0];
      const reader = new FileReader();
      reader.onload = (event) => {
        if (type === "key") {
          key = event.target.result as string;
        } else {
          iv = event.target.result as string;
        }
      };
      reader.readAsText(file);
    };
    input.click();
  }
</script>

<form on:submit|preventDefault={handleSubmit}>
  <h2>Dešifriranje</h2>
  <div class="form-input">
    <label for="file">Datoteka</label>
    <br />
    <input type="file" on:change={handleFileChange} name="file" />
  </div>
  <br />
  <div class="form-input">
    <label for="key">Ključ</label>
    <br />
    <input type="text" bind:value={key} />
    <button on:click={handleKeyImport} type="button" class="importBtn">Uvozi</button>
  </div>
  <br />
  <div class="form-input">
    <label for="iv">IV</label>
    <br />
    <input type="text" bind:value={iv} />
    <button on:click={handleIvImport} type="button" class="importBtn">Uvozi</button>
  </div>
  <br />
  <button type="submit">Dešifriraj</button>
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
  .importBtn {
    margin-left: 10px;
  }
</style>
