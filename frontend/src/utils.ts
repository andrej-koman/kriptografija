import forge from 'node-forge';

export const generateKeyPair = (keySize: number, fileName: string): {
  privateKey: File,
  publicKey: File
} => {
  // Generate an RSA key pair in PEM format
  const keys = forge.pki.rsa.generateKeyPair({ bits: keySize });

  // Convert the private and public keys to strings
  const privateKeyPem = forge.pki.privateKeyToPem(keys.privateKey);
  const publicKeyPem = forge.pki.publicKeyToPem(keys.publicKey);

  // Create a Blob object from the private key string
  const privateKeyBlob = new Blob([privateKeyPem], { type: 'text/plain;charset=utf-8' });

  // Create a File object from the Blob object and the file name
  const privateKeyFile = new File([privateKeyBlob], fileName);

  // Create a Blob object from the public key string
  const publicKeyBlob = new Blob([publicKeyPem], { type: 'text/plain;charset=utf-8' });

  // Create a File object from the Blob object and the file name
  const publicKeyFile = new File([publicKeyBlob], fileName);

  return {
    privateKey: privateKeyFile,
    publicKey: publicKeyFile
  };
};

export const downloadTextFile = (file: File, fileName: string): void => {
  // Create a link element
  const element = document.createElement('a');

  // Set the link's href to the Blob URL
  const blob = new Blob([file], { type: 'text/plain' }); 

  // Set the link's download attribute
  element.href = URL.createObjectURL(blob);
  element.download = fileName;

  // Trigger the download by simulating click
  document.body.appendChild(element); // Required for this to work in FireFox
  element.click();

  // Revoke the Object URL to avoid memory leaks
  URL.revokeObjectURL(element.href);
}

export const downloadFile = (file: File, originalFileName: string): void => { 
  // Create a link element
  const element = document.createElement('a');

  // Set the link's href to the Blob URL
  const blob = new Blob([file], { type: 'application/octet-stream' }); 

  // Set the link's download attribute
  element.href = URL.createObjectURL(blob);
  element.download = `${originalFileName.split('.').shift()}_encrypted.${originalFileName.split('.').pop()}`;

  // Trigger the download by simulating click
  document.body.appendChild(element); // Required for this to work in FireFox
  element.click();

  // Revoke the Object URL to avoid memory leaks
  URL.revokeObjectURL(element.href);
}
