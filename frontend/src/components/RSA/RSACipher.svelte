<script lang="ts">
  import axios from "axios";
  import { generateKeyPair, downloadTextFile, downloadFile } from "../../utils";

  let keyLength: number, file: File;

  const handleSubmit = async () => {
    // Generate the key pair
    const { publicKey, privateKey } = generateKeyPair(keyLength, file.name);

    // Create the form data
    const formData = new FormData();
    formData.append("file", file);
    formData.append("publicKey", publicKey);
    formData.append("keySize", keyLength.toString());

    // Send the request
    try {
      const response = await axios.post(
        "http://localhost:5064/rsa/encrypt",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
          responseType: "blob",
        }
      );

      // Download the encrypted file
      downloadFile(response.data, file.name);

      // Get the encrypted key from header
      const encryptedKey = response.headers["encryptedkey"];

      // Download the encrypted key as text file
      downloadTextFile(
        encryptedKey,
        file.name.split(".").shift() + "_encryptedKey.txt"
      );

      // Get the iv from header
      const iv = response.headers["iv"];

      // Download the iv as text file
      downloadTextFile(iv, file.name.split(".").shift() + "_iv.txt");
    } catch (error) {
      console.error(error);
    } finally {
      // Download the private key
      downloadTextFile(
        privateKey,
        file.name.split(".").shift() + "_privateKey.txt"
      );
    }
  };

  function handleFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input) {
      file = input.files[0];
    }
  }
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
      <option value="1024">1024 bitov</option>
      <option value="2048">2048 bitov</option>
    </select>
  </div>
  <br />
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
</style>
