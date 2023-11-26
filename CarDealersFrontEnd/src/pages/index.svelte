<script lang="ts">
  import { navigateOnStart } from "../lib/shared";

  let numDots = 1;
  setInterval(
    () => numDots = numDots === 3 ? 1 : numDots + 1,
    1000
  );
  
  navigateOnStart()
    ?.then(status => {
      if (status === 'SessionExists') {
        window.location.href = '/home'
        return;
      }
      window.location.href = '/login'
    });
</script>

<div> Loading{'.'.repeat(numDots)} </div>