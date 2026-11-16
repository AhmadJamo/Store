# Staged inter-branch transfer receipts

## Decision

Approved transfers reserve source inventory. Dispatch is a separate, source-branch-authorized transition to InTransit. Destination receipt moves only accepted quantities from source to destination, consumes the matching reservation quantity and records exact cumulative received quantity. Short and damaged quantities are explicit open evidence and never create an automatic inventory loss or accounting entry.

Dispatch requires two different assigned branches, a verified source branch context and AllowTransferOut. Receipt requires the verified destination branch context and AllowTransferIn. Dispatch and receipt have independent permissions and antiforgery protection.

## Consequences

Partial receipts are repeatable and cannot exceed the outstanding quantity. A transfer becomes Received only when every ordered unit is accepted. Reported discrepancy keeps the transfer in ReceiptException so later operational and accounting resolution remains explicit. Historic Posted and Cancelled transfers remain readable and keep their former reversal behavior.
