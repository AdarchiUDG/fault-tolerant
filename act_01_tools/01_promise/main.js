import axios from 'axios'

axios.get("https://jsonplaceholder.typicode.com/todos/1")
    .then(value => {
        console.log("Got a response:")
        console.table(value.data)
    })
    .catch(reason => {
        console.error("Got an error:")
        console.error(reason.toString())
    })
    .finally(() => console.log("\nAlways prints"));

