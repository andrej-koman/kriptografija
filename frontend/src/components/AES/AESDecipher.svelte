<!-- <script lang="ts">
    let fileString: any;
    let key: string;
    let keySize: number;
    let mode: string;
    let IV: string;
  
    const onFileInput = (e: Event): void => {
      const file = (e.target as HTMLInputElement).files[0];
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        const text = e.target?.result;
        if (text) {
          fileString = e.target?.result;
        }
      };
      reader.readAsBinaryString(file);
    };

    const encrypt = () => {
        //TODO - Start implementing
    }
  </script>
  
  <div class="decipher">
    <h2>Dešifriraj</h2>
    <div class="form-input">
      <h3>Datoteka</h3>
      <input type="file" accept=".zip" on:input={onFileInput} />
    </div>
    <div class="form-input">
      <h3>Način</h3>
      <select bind:value={mode}>
        <option value="CBC">CBC</option>
        <option value="ECB">ECB</option>
      </select>
    </div>
    <div class="form-input">
      <h3>Velikost ključa</h3>
      <select bind:value={keySize}>
        <option value="128">128 bitov</option>
        <option value="192">192 bitov</option>
        <option value="256">256 bitov</option>
      </select>
    </div>
    <div class="form-input">
      <h3>Šifrirni ključ</h3>
      <input type="text" bind:value={key} />
    </div>
    <div class="form-input">
      <h3>IV (Initialization vector)</h3>
      <input type="text" bind:value={IV} />
    </div>
    <div class="btn">
      <button on:click={encrypt}>Šifriraj</button>
    </div>
  </div>
  <div class="error"></div>

  
  <style>
      .form-input {
          display: flex;
          flex-direction: column;
          margin-bottom: 1rem;
          text-align: start;
      }
      .form-input > h3 {
          margin-bottom: 0.5rem;
      }
      .decipher {
          margin: 20px;
      }
  </style>
   -->