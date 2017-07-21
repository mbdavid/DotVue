<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public void Upload(HttpPostedFile file1)
        {
            ClientScript.Alert(file1?.ContentLength.ToString() ?? "no-file");
        }

        public void Upload2(HttpPostedFile fileA, HttpPostedFile fileB)
        {
            ClientScript.Alert(fileA.ContentLength + " - " + fileB.ContentLength);
        }

        public void UploadMultiple(IList<HttpPostedFile> files)
        {
            ClientScript.Alert("Total: " + files.Count);
        }

        public void UploadAndParams(int a, IList<HttpPostedFile> files, string b)
        {
            ClientScript.Alert("Total: " + files.Count + "(a=" + a + "-b=" + b + ")");

            //JS.Code("this.$refs.fileP.value=null");
        }
    }

</script>
<template>
    <div>
        <h3>Simple Upload file</h3>

        <hr />
        <input type="file" ref="file1" />
        <button @click="Upload($refs.file1)">Upload Single File</button>

        <hr />
        <input type="file" ref="fileA" />
        <input type="file" ref="fileB" />
        <button @click="Upload2($refs.fileA, $refs.fileB)">Upload A & B</button>

        <hr />
        <input type="file" ref="fileM" multiple />
        <button @click="UploadMultiple($refs.fileM)">Upload Multiples</button>

        <hr />
        <input type="file" ref="fileP" multiple />
        <button @click="UploadAndParams(65, $refs.fileP, 'B')">Upload With Params</button>

    </div>
</template>