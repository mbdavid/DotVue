@page /upload

<div>
    <h3>Simple Upload file</h3>

    <hr />
    <input type="file" ref="file1" />
    <button @click="upload1($refs.file1)">Upload Single File</button>

    <hr />
    <input type="file" ref="fileA" />
    <input type="file" ref="fileB" />
    <button @click="upload2($refs.fileA, $refs.fileB)">Upload A & B</button>

    <hr />
    <input type="file" ref="fileM" multiple />
    <button @click="uploadMultiple($refs.fileM)">Upload Multiples</button>

    <hr />
    <input type="file" ref="fileP" multiple />
    <button @click="uploadAndParams(65, $refs.fileP, 'B')">Upload With Params</button>

</div>