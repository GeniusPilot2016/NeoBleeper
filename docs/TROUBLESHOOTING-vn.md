# Hướng dẫn Khắc phục Sự cố NeoBleeper

Hướng dẫn này cung cấp giải pháp cho các sự cố thường gặp khi sử dụng NeoBleeper, đặc biệt là các sự cố liên quan đến hoạt động của loa hệ thống, đầu ra âm thanh, khả năng tương thích phần cứng và tiếng bíp liên tục của hệ thống.

---

## 1. Âm thanh bị kẹt trong loa hệ thống sau khi gặp sự cố hoặc bị đóng mạnh

**Sự cố:**
Nếu NeoBleeper gặp sự cố hoặc bị đóng mạnh trong khi âm thanh đang phát qua loa hệ thống (PC), âm thanh có thể bị "kẹt", dẫn đến tiếng bíp hoặc tiếng ù liên tục.

**Nguyên nhân xảy ra:**
Loa hệ thống được điều khiển ở mức phần cứng/phần mềm thấp. Nếu ứng dụng không nhả hoặc đặt lại loa đúng cách khi thoát, tiếng bíp có thể vẫn tiếp diễn.

**Giải pháp:**
- **Sử dụng tiện ích NeoBleeper Nút Chặn Tiếng Bíp (cho phiên bản 64-bit):**
NeoBleeper đi kèm với một công cụ có tên "NeoBleeper Nút Chặn Tiếng Bíp" trong thư mục chương trình.

![image4](https://github.com/user-attachments/assets/3e46c8dd-62b9-4789-a7a0-ca245d365dce)

- Khởi chạy công cụ này và nhấn nút **Dừng tiếng bíp** để dừng tiếng bíp bị kẹt từ loa hệ thống.
- Chỉ sử dụng tiện ích này khi tiếng bíp vẫn tiếp tục sau khi gặp sự cố hoặc buộc thoát.

  #### Thông báo Dừng tiếng bíp và Ý nghĩa của chúng
  
    Khi sử dụng tiện ích Dừng tiếng bíp, bạn có thể thấy các thông báo sau:

  ![image1](https://github.com/user-attachments/assets/3ff161a2-ede8-4ce8-a10b-26eb8a0d7220)
    
    **Loa hệ thống không phát ra tiếng bíp hoặc loa hệ thống phát ra tiếng bíp theo cách khác. Không có hành động nào được thực hiện.**  
    Thông báo này xuất hiện khi tiện ích kiểm tra loa hệ thống và xác định loa không phát ra tiếng bíp hoặc phát ra tiếng bíp theo cách mà công cụ không thể điều khiển. Trong trường hợp này, Nút Chặn Tiếng Bíp sẽ không thực hiện bất kỳ hành động nào nữa.
    - *Mẹo:* Nếu bạn vẫn nghe thấy tiếng bíp liên tục, hãy thử khởi động lại máy tính.

  ![image2](https://github.com/user-attachments/assets/98b1e3ea-bb53-4181-bb7f-a302d45079ab)
    
    **Tiếng bíp đã dừng thành công!**  
    Thông báo này xác nhận rằng tiện ích Nút Chặn Tiếng Bíp đã phát hiện tiếng bíp bị kẹt và đã dừng thành công. Không cần thực hiện thêm thao tác nào nữa.

  ![image3](https://github.com/user-attachments/assets/6c1b890f-db60-48b4-84f0-531b00918a0a)
  
    **Không có đầu ra loa hệ thống hoặc đầu ra loa hệ thống không chuẩn. Bộ chặn tiếng bíp có thể gây mất ổn định hoặc các hiện tượng không mong muốn. Bạn có muốn tiếp tục không??**  
    Thông báo này xuất hiện khi tiện ích Nút Chặn Tiếng Bíp được khởi động và phát hiện hệ thống của bạn không có loa hệ thống chuẩn (PC) hoặc đầu ra loa hệ thống "không chuẩn". Trong trường hợp này, tiện ích cảnh báo bạn rằng việc cố gắng sử dụng Nút Chặn Tiếng Bíp có thể không hoạt động như mong đợi và có khả năng gây ra hành vi bất ngờ hoặc mất ổn định.

    Nếu bạn tiếp tục, công cụ sẽ cố gắng dừng tiếng bíp, nhưng việc này có thể không hiệu quả hoặc gây ra tác dụng phụ nếu phần cứng của bạn không được hỗ trợ hoặc không chuẩn.
    
    Nếu bạn chọn không tiếp tục, công cụ sẽ thoát mà không thực hiện bất kỳ thay đổi nào.
    - *Mẹo:* Nếu bạn nhận được thông báo này, điều đó có nghĩa là máy tính của bạn không có loa hệ thống tương thích hoặc đầu ra của loa không thể được kiểm soát một cách đáng tin cậy. Bất kỳ tiếng bíp hoặc tiếng ù nào bạn nghe thấy có thể đến từ một thiết bị âm thanh khác (chẳng hạn như loa chính hoặc tai nghe của bạn). Hãy sử dụng cài đặt thiết bị âm thanh tiêu chuẩn của bạn để giải quyết các sự cố âm thanh và đóng bất kỳ ứng dụng nào có thể đang tạo ra âm thanh không mong muốn. Nếu sự cố vẫn tiếp diễn, hãy thử khởi động lại máy tính hoặc kiểm tra cài đặt âm thanh của thiết bị.
      
- **Khởi động lại máy tính:**
  Nếu Nút Chặn Tiếng Bíp không giải quyết được sự cố, việc khởi động lại hệ thống sẽ thiết lập lại phần cứng loa.

- **Phòng ngừa:**
  Luôn đóng NeoBleeper một cách bình thường. Tránh buộc đóng nó bằng Trình quản lý tác vụ hoặc các công cụ tương tự khi âm thanh đang phát.

---

## 2. Phát hiện và Tương thích Loa Hệ thống
NeoBleeper bao gồm logic phát hiện để kiểm tra xem hệ thống của bạn có đầu ra loa PC tiêu chuẩn hay không, cũng như hỗ trợ đầu ra loa hệ thống "ẩn" (chẳng hạn như những đầu ra không sử dụng ID PNP0800). Nếu phần cứng của bạn không hỗ trợ loa hệ thống tiêu chuẩn hoặc ẩn, hoặc đầu ra không chuẩn và không sử dụng được, bạn có thể thấy thông báo cảnh báo hoặc phải dựa vào thiết bị âm thanh thông thường để phát ra tiếng bíp. Tuy nhiên, bắt đầu từ các phiên bản gần đây, NeoBleeper không còn bắt buộc bạn phải sử dụng thiết bị âm thanh độc quyền khi không có loa tiêu chuẩn nữa—giờ đây, nó cho phép sử dụng đầu ra loa hệ thống ẩn/không phải PNP0800 nếu có.

### Ví dụ cảnh báo (Hình 1):

![image1](https://github.com/user-attachments/assets/45efe32d-1253-4b8f-a8f1-aa8cbcd22853)

> **Giải thích:**
> Bo mạch chủ máy tính của bạn không có đầu ra loa hệ thống tiêu chuẩn, hoặc đầu ra không chuẩn. NeoBleeper sẽ cố gắng phát hiện và đề xuất sử dụng các đầu ra loa hệ thống "ẩn" không được xác định là PNP0800. Nếu có đầu ra như vậy, giờ đây bạn có thể sử dụng loa hệ thống ngay cả khi cảnh báo này xuất hiện. Nếu không, NeoBleeper sẽ chuyển sang thiết bị âm thanh thông thường của bạn (như loa hoặc tai nghe).

### Hộp thoại Cài đặt (Hình ảnh 2 và 3):

![image2](https://github.com/user-attachments/assets/eb286365-b9e5-4e97-a317-809513518c0c)

![image3](https://github.com/user-attachments/assets/a727f9e1-c2b0-4979-bd5e-c82a8f840446)

- **Tính khả dụng của nút "Kiểm tra Loa Hệ thống":**
  Tùy chọn này được bật nếu NeoBleeper phát hiện bất kỳ đầu ra loa hệ thống nào có thể sử dụng được, bao gồm cả đầu ra ẩn hoặc không phải PNP0800.
- **Cài đặt "Sử dụng thiết bị âm thanh để tạo tiếng bíp":**
  Giờ đây, bạn được phép tắt tính năng này nếu phát hiện đầu ra loa hệ thống ẩn hoặc không chuẩn.

#### "Đầu ra loa hệ thống không chuẩn" nghĩa là gì?
Một số máy tính, máy tính xách tay hoặc máy ảo hiện đại không có loa PC thực sự, hoặc đường truyền tín hiệu không chuẩn. NeoBleeper hiện đang cố gắng phát hiện và sử dụng các đầu ra loa hệ thống ẩn này (không được xác định là thiết bị PNP0800), nhưng chỉ có thể bật tùy chọn loa hệ thống nếu nó thực sự có thể truy cập được ở cấp độ phần cứng. Nếu không tìm thấy đầu ra nào khả dụng, bạn sẽ cần sử dụng thiết bị âm thanh thông thường của mình.

## 2.1 Kiểm tra Đầu ra Loa Hệ thống (Phát hiện Tần số Siêu âm)
NeoBleeper hiện bao gồm một bài kiểm tra phần cứng mới, nâng cao để phát hiện đầu ra loa hệ thống (hay còn gọi là loa PC), ngay cả khi thiết bị không được Windows báo cáo (trong một số ID nhất định như PNP0C02 thay vì PNP0800). Bài kiểm tra này sử dụng tần số siêu âm (thường là 30–38 kHz, không thể nghe thấy) và phân tích phản hồi điện trên cổng loa hệ thống.

- **Cách thức hoạt động:**
  Trong quá trình khởi động, NeoBleeper thực hiện bước thứ hai sau khi kiểm tra ID thiết bị thông thường. Nó gửi tín hiệu siêu âm đến cổng loa hệ thống và theo dõi phản hồi phần cứng để phát hiện sự hiện diện của đầu ra loa đang hoạt động—ngay cả khi bị ẩn hoặc không chuẩn.

- **Những điều bạn có thể nhận thấy:**
  Trên một số hệ thống, đặc biệt là những hệ thống có còi áp điện, bạn có thể nghe thấy tiếng lách cách nhỏ trong giai đoạn này. Điều này là bình thường và cho biết bài kiểm tra phần cứng đang chạy.
  ![image4](https://github.com/user-attachments/assets/81a8d6d8-565e-4fd3-aefc-6122c06d17a9)
  *Đang kiểm tra sự hiện diện của đầu ra loa hệ thống (loa máy tính) ở bước 2/2... (bạn có thể nghe thấy tiếng tách tách)*

- **Tại sao phải kiểm tra?**
  Nhiều hệ thống hiện đại không có thiết bị loa hệ thống PNP0800, nhưng vẫn có đầu ra loa có thể sử dụng được (ẩn). NeoBleeper sử dụng phương pháp tiên tiến này để bật tính năng phát tiếng bíp trên nhiều phần cứng hơn.

---

## 3. Hỗ trợ và Hạn chế ARM64

**Thiết bị chạy ARM64:**
Trên các hệ thống Windows ARM64, tính năng kiểm tra "Loa Hệ thống" và hộp kiểm "Sử dụng thiết bị âm thanh để tạo tiếng bíp" **không khả dụng** trong NeoBleeper. Thay vào đó, tất cả tiếng bíp và đầu ra âm thanh luôn được tạo ra thông qua thiết bị âm thanh tiêu chuẩn của bạn (loa hoặc tai nghe).

- Nút "Kiểm tra Loa Hệ thống" và các tính năng phát hiện liên quan sẽ **không** hiển thị trong phần cài đặt trên các thiết bị ARM64.
- Tùy chọn "Sử dụng thiết bị âm thanh để tạo tiếng bíp" không có sẵn vì hành vi này được áp dụng tự động.
- Hạn chế này tồn tại vì không thể truy cập trực tiếp vào phần cứng loa máy tính/hệ thống trên nền tảng ARM64 Windows.
- Bạn sẽ luôn nghe thấy tiếng bíp thông qua thiết bị đầu ra âm thanh thông thường trên ARM64.

**Nếu bạn đang sử dụng máy ARM64 và không thấy các tùy chọn loa hệ thống trong NeoBleeper, điều này là bình thường và không phải là lỗi.**

---

## 4. Cách kiểm tra sự hiện diện của loa hệ thống

- **Máy tính để bàn:** Hầu hết các máy tính để bàn cũ đều có đầu cắm loa PC trên bo mạch chủ. Các hệ thống mới hơn có thể bỏ qua tính năng này hoặc có thể hiển thị đầu ra ở dạng ẩn/không phải PNP0800 mà NeoBleeper hiện có thể sử dụng.
- **Máy tính xách tay:** Hầu hết máy tính xách tay không có loa hệ thống riêng; tất cả âm thanh đều được định tuyến qua hệ thống âm thanh chính.
- **Máy ảo:** Tính năng mô phỏng loa hệ thống thường không có hoặc không đáng tin cậy; các đầu ra không phải PNP0800 có thể không khả dụng.
- **Cách nhận biết:** Nếu bạn thấy các cảnh báo ở trên nhưng có thể bật và kiểm tra loa hệ thống trong NeoBleeper, máy tính của bạn có thể có đầu ra ẩn hoặc không chuẩn.

---

## 5. Tôi không nghe thấy âm thanh nào!
- **Kiểm tra cài đặt NeoBleeper của bạn:**
  Nếu loa hệ thống của bạn không khả dụng, hãy đảm bảo thiết bị âm thanh (loa/tai nghe) được chọn chính xác và đang hoạt động.
- **Kiểm tra bộ trộn âm lượng của Windows:**
  Đảm bảo NeoBleeper không bị tắt tiếng trong bộ trộn âm lượng của hệ thống.
- **Hãy thử nút "Kiểm tra loa hệ thống":**
  Sử dụng nút này để kiểm tra loa máy tính của bạn.
- **Đọc thông báo cảnh báo:**
  NeoBleeper sẽ cung cấp hướng dẫn cụ thể nếu không thể truy cập loa hệ thống của bạn.

---

## 6. Câu hỏi thường gặp

### H: Tôi có thể sử dụng loa hệ thống nếu phần cứng của tôi không có thiết bị PNP0800 không?
**A:** Có! NeoBleeper hiện đang cố gắng phát hiện và sử dụng các đầu ra loa hệ thống ẩn hoặc không phải PNP0800 nếu có thể. Nếu thành công, bạn có thể sử dụng loa hệ thống ngay cả khi Windows không báo cáo thiết bị tiêu chuẩn.

### H: Tại sao cài đặt "Sử dụng thiết bị âm thanh để tạo tiếng bíp" đôi khi trở thành cài đặt cố định (trong các phiên bản cũ hơn)?
**A:** Khi không phát hiện thấy đầu ra loa hệ thống tiêu chuẩn (trong các phiên bản cũ hơn), NeoBleeper sẽ áp dụng cài đặt này để đảm bảo vẫn có thể phát ra âm thanh.

### H: Có cách nào khắc phục tình trạng loa hệ thống bị mất không?
**A:** Bạn phải sử dụng thiết bị âm thanh thông thường (loa/tai nghe) nếu không tìm thấy đầu ra loa hệ thống tiêu chuẩn (trong các phiên bản cũ hơn).

### H: Điều gì sẽ xảy ra nếu công cụ Chặn Tiếng Bíp không dừng được tiếng bíp bị kẹt?
**A:** Khởi động lại máy tính để thiết lập lại phần cứng loa nếu tiện ích Nút Chặn Tiếng Bíp bị lỗi.

### H: Tại sao tôi nghe thấy tiếng lách cách khi khởi động?
**A:** Trong quá trình kiểm tra đầu ra loa hệ thống nâng cao (bước 2), NeoBleeper gửi tín hiệu siêu âm đến phần cứng để phát hiện các đầu ra loa ẩn hoặc không chuẩn. Trên một số hệ thống (đặc biệt là những hệ thống có còi áp điện), điều này có thể gây ra tiếng lách cách yếu ớt. Điều này là bình thường và không phải là dấu hiệu của sự cố; nó chỉ đơn giản có nghĩa là bài kiểm tra phần cứng đang chạy.

### H: Bài kiểm tra phần cứng siêu âm (bước 2) có thể phát hiện loa hệ thống bị hỏng (hở mạch) hoặc bị ngắt kết nối không?
**A:** Hiện tại, điều này chưa được kiểm tra và chưa rõ. Mặc dù bài kiểm tra này kiểm tra phản hồi điện và hoạt động của cổng, nhưng nó có thể không phân biệt chính xác giữa loa hiện diện nhưng bị hỏng (hở mạch) hoặc bị ngắt kết nối và loa bị mất. Nếu loa bị hỏng hoàn toàn hoặc bị ngắt kết nối (hở mạch), bài kiểm tra có thể trả về kết quả sai, cho biết không phát hiện thấy đầu ra hoạt động. Tuy nhiên, kết quả này không được đảm bảo và có thể phụ thuộc vào phần cứng và chế độ lỗi cụ thể. Nếu bạn nghi ngờ loa hệ thống của mình không hoạt động, hãy kiểm tra thực tế hoặc sử dụng đồng hồ vạn năng.

### H: Tại sao tôi không thấy bất kỳ tùy chọn loa hệ thống hoặc âm thanh bíp nào trên thiết bị ARM64 của mình?
**A:** Trên hệ thống Windows ARM64, NeoBleeper vô hiệu hóa các cài đặt liên quan đến loa hệ thống vì nền tảng ARM64 không hỗ trợ truy cập trực tiếp vào phần cứng loa hệ thống. Tất cả tiếng bíp đều được phát qua thiết bị đầu ra âm thanh thông thường của bạn (loa hoặc tai nghe), và các tùy chọn "Kiểm tra Loa Hệ thống" và "Sử dụng thiết bị âm thanh để tạo tiếng bíp" sẽ tự động bị ẩn. Hiện tượng này là do thiết kế chứ không phải lỗi.

**Các bản cập nhật tiềm năng trong tương lai:**
Nếu các thử nghiệm hoặc phát triển trong tương lai cho phép NeoBleeper phát hiện loa hệ thống bị hỏng hoặc ngắt kết nối một cách đáng tin cậy thông qua kiểm tra phần cứng siêu âm, Câu hỏi thường gặp này và logic phát hiện sẽ được cập nhật để phản ánh những cải tiến đó. Vui lòng theo dõi nhật ký thay đổi hoặc bản phát hành mới để biết thêm chi tiết.

---

## 7. Nhận trợ giúp

- **Cung cấp thông tin chi tiết về máy tính và môi trường:** Khi báo cáo sự cố phát hiện phần cứng hoặc âm thanh, vui lòng bao gồm thông tin chi tiết về máy tính của bạn (máy tính để bàn/máy tính xách tay, nhà sản xuất/mẫu máy, hệ điều hành) và bất kỳ phần cứng nào có liên quan.
- **Đính kèm ảnh chụp màn hình hoặc hộp thoại báo lỗi:** Ảnh chụp màn hình hộp thoại báo lỗi hoặc cảnh báo rất hữu ích. Vui lòng chỉ rõ thời điểm sự cố xảy ra.
- **Bao gồm tệp nhật ký:** Bắt đầu từ các phiên bản mới hơn, NeoBleeper sẽ tạo một tệp nhật ký chi tiết có tên `DebugLog.txt` trong thư mục chương trình. Vui lòng đính kèm tệp này khi cần trợ giúp, vì tệp này chứa thông tin chẩn đoán hữu ích.
- **Mô tả các bước để tái tạo sự cố:** Nêu rõ những gì bạn đang làm khi sự cố xảy ra.
- **Mở một sự cố trên GitHub:** Để được hỗ trợ thêm, hãy mở một sự cố trên GitHub và bao gồm tất cả các chi tiết trên để được hỗ trợ tốt nhất.

_Hướng dẫn này được cập nhật khi phát hiện ra các sự cố và giải pháp mới. Để được hỗ trợ thêm, vui lòng mở một sự cố trên GitHub kèm theo thông tin chi tiết về thiết lập của bạn và sự cố gặp phải._
