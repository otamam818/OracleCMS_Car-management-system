<script lang="ts">
  import { handleSignin } from "../../lib/api";
  let username = "";
  let password = "";
  let message = "";
</script>

<div class="form">
  <h1>Login</h1>
  <input
    class="input"
    type="text"
    placeholder="Username"
    bind:value={username}
  />
  <input
    class="input"
    type="password"
    placeholder="Password"
    bind:value={password}
  />
  <button class="button" on:click={() => handleSignin(username, password)}>
    Submit
  </button>
  {#if message}
    <p class="message">{message}</p>
  {/if}
</div>

<style>
  .form {
    width: 300px;
    margin: 0 auto;
  }

  .input {
    display: block;
    width: 100%;
    padding: 10px;
    margin: 10px 0;
    border: 1px solid #ccc;
  }

  .button {
    display: block;
    width: 100%;
    padding: 10px;
    margin: 10px 0;
    background: #007bff;
    color: #fff;
    border: none;
    cursor: pointer;
  }

  .message {
    color: #ff0000;
    text-align: center;
  }
</style>
