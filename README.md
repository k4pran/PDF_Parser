# Prickle Parser

Extracts location aware content from PDF files leveraging IText7. Content extracted is broken down into book, pages, 
lines, chunks, words and chars. Each contain various metrics such as locations, widths, baseline, descent etc. 

### Progress:
  - [ ] Handle edge cases for determining where space characters should be inserted
  - [ ] Handle images better as many images are not being extracted correctly by IText