CREATE TRIGGER TriggerWishlistUpdate
ON Wishlist
AFTER UPDATE
AS
BEGIN
    IF UPDATE(WishlistName)
    BEGIN
        UPDATE Wishlist
        SET UpdatedAt = GETDATE()
        FROM Wishlist
        INNER JOIN inserted ON Wishlist.WishlistId = inserted.WishlistId;
    END
END;