use serde_json;

async fn request(url: String) -> Result<serde_json::Value, reqwest::Error> {
    let request_result = reqwest::get(url)
        .await;

    let response = match request_result {
        Ok(r) => r,
        Err(e) => return Err(e),
    };

    let json_result = response.json::<serde_json::Value>()
        .await;

    json_result
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let json_result = request("https://jsonplaceholder.typicode.com/todos/1".to_string())
        .await;
    
    match json_result {
        Ok(r) => println!("{:#?}", r),
        Err(e) => println!("{}", e)
    };

    println!("Always prints");

    Ok(())
}